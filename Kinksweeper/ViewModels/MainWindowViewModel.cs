using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Kinksweeper.Models;
using ReactiveUI;

namespace Kinksweeper.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        private static FieldConfiguration currentFieldConfiguration = new();

        private InternalMineField? _mineField;
        private Button[,]? _buttons;
        private KinkStorage? _kinkStorage;

        private HashSet<PictureContainer>? _selectedPunishments;

        public Interaction<PunishmentViewModel, PictureContainer?> ShowPunishmentDialog { get; }
        private ICommand PunishmentCommand { get; }

        public Interaction<EndgameViewModel, Unit> ShowEndGameDialog { get; }
        private ICommand EndGameCommand { get; }

        public ReactiveCommand<Unit, Unit> RestartCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectCustomConfigurationCommand { get; }
        public Interaction<FieldConfiguratorViewModel, FieldConfiguration?> ShowCustomConfigurationWindow { get; }
        
        public ReactiveCommand<Unit, Unit> SelectEasyConfiguration { get; }
        public ReactiveCommand<Unit, Unit> SelectMediumConfiguration { get; }
        public ReactiveCommand<Unit, Unit> SelectHardConfiguration { get; }


        public MainWindowViewModel()
        {
            ShowPunishmentDialog = new Interaction<PunishmentViewModel, PictureContainer?>();
            PunishmentCommand = ReactiveCommand.Create(async (string kink) =>
            {
                var store = new PunishmentViewModel(kink, currentFieldConfiguration.PicsPerPunishment);

                var result = await ShowPunishmentDialog.Handle(store);
                if (result is not null)
                {
                    _selectedPunishments!.Add(result);
                }
                
                if (_mineField!.Finished())
                {
                    EndGameTriggered();
                }
            });
            
            SelectEasyConfiguration = ReactiveCommand.Create(() =>
            {
                currentFieldConfiguration = Constants.EasyConfiguration;
                InitializeGame();
            });
            
            SelectMediumConfiguration = ReactiveCommand.Create(() =>
            {
                currentFieldConfiguration = Constants.MediumConfiguration;
                InitializeGame();
            });
            
            SelectHardConfiguration = ReactiveCommand.Create(() =>
            {
                currentFieldConfiguration = Constants.HardConfiguration;
                InitializeGame();
            });
            

            ShowCustomConfigurationWindow = new Interaction<FieldConfiguratorViewModel, FieldConfiguration?>();
            SelectCustomConfigurationCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var model = new FieldConfiguratorViewModel();
                var result = await ShowCustomConfigurationWindow.Handle(model);
                if (result is not null)
                {
                    currentFieldConfiguration = result;
                    InitializeGame();
                }
            });
            
            ShowEndGameDialog = new Interaction<EndgameViewModel, Unit>();
            EndGameCommand = ReactiveCommand.CreateFromTask(async (HashSet<PictureContainer> kinks) =>
            {
                var model = new EndgameViewModel(kinks);
                await ShowEndGameDialog.Handle(model);
                InitializeGame();
            });

            RestartCommand = ReactiveCommand.Create(InitializeGame);
        }

        private Viewbox GeneratePositionView(int row, int col)
        {
            var btn = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Height = 32, Width = 32,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            btn.Click += (_, _) => { ProcessLeftMouseClick(row, col); };
            btn.PointerReleased += (_, args) => { ProcessRightMouseClick(args, row, col); };

            _buttons![row, col] = btn;

            var viewbox = new Viewbox
            {
                StretchDirection = StretchDirection.Both,
                Stretch = Stretch.Fill,
                
            };
            Grid.SetRow(viewbox, row);
            Grid.SetColumn(viewbox, col);
            viewbox.Child = btn;
            return viewbox;
        }

        public void InitializeGame()
        {
            _kinkStorage = new KinkStorage();
            _selectedPunishments = new HashSet<PictureContainer>();

            var grid = App.MainWindow!._MineField._MineGrid!;
            grid.Children.Clear();
            grid.ColumnDefinitions = ColumnDefinitions.Parse(string.Join(',', Enumerable.Repeat("1*", currentFieldConfiguration.Dimension)));
            grid.RowDefinitions = RowDefinitions.Parse(string.Join(',', Enumerable.Repeat("1*", currentFieldConfiguration.Dimension)));

            _mineField = new InternalMineField(currentFieldConfiguration.Dimension, currentFieldConfiguration.Dimension, currentFieldConfiguration.MinesCount);
            _buttons = new Button[currentFieldConfiguration.Dimension, currentFieldConfiguration.Dimension];

            for (var i = 0; i < currentFieldConfiguration.Dimension * currentFieldConfiguration.Dimension; i++)
            {
                var cur_row = i / currentFieldConfiguration.Dimension;
                var cur_col = i % currentFieldConfiguration.Dimension;
                var obj = GeneratePositionView(cur_row, cur_col);
                grid.Children.Add(obj);
            }
        }

        private void ProcessRightMouseClick(PointerReleasedEventArgs args, int row, int col)
        {
            if (args.InitialPressMouseButton != MouseButton.Right)
            {
                return;
            }

            var position = _mineField![row, col];
            var btn = _buttons![row, col];
            switch (position.state)
            {
                case PositionState.CLOSED:
                    position.state = PositionState.FLAGGED;
                    var flagAsset = AvaloniaLocator.Current.GetService<IAssetLoader>()?.Open(new Uri("avares://Kinksweeper/Assets/redflag.png"));
                    btn.Content = flagAsset is not null ? new Image { Source = new Bitmap(flagAsset)} : "!";
                    break;
                case PositionState.FLAGGED:
                    position.state = PositionState.CLOSED;
                    btn.Content = "";
                    break;
            }
        }

        private void ProcessLeftMouseClick(int row, int col)
        {
            var position = _mineField![row, col];
            if (position.state != PositionState.CLOSED)
            {
                return;
            }

            if (position.hasMine)
            {
                position.state = PositionState.OPEN;
                var mineAsset = AvaloniaLocator.Current.GetService<IAssetLoader>()?.Open(new Uri("avares://Kinksweeper/Assets/failure.png"));
                _buttons![row, col].Content = mineAsset is not null ? new Image { Source = new Bitmap(mineAsset) } : "*";
                MineTriggered();
                return;
            }

            var positionsToOpen = _mineField.ReactToOpenField(row, col);
            foreach (var tuple in positionsToOpen)
            {
                _mineField[tuple.Item1, tuple.Item2].state = PositionState.OPEN;
                _buttons![tuple.Item1, tuple.Item2].Content = _mineField[tuple.Item1, tuple.Item2].minesAround;
            }

            if (_mineField.Finished())
            {
                EndGameTriggered();
            }
        }

        private void MineTriggered()
        {
            if (_kinkStorage!.IsStorageEmpty())
            {
                EndGameTriggered();
                return;
            }

            PunishmentCommand.Execute(_kinkStorage.PopRandomKink());
        }

        private void EndGameTriggered()
        {
            EndGameCommand.Execute(_selectedPunishments);
        }
    }
}