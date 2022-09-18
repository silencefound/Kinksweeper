using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Kinksweeper.Models;
using Kinksweeper.ViewModels;
using ReactiveUI;

namespace Kinksweeper.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MineField _MineField => MineField;
        public MainWindow()
        {
            Opened += OnOpened;
            InitializeComponent();
            this.WhenActivated(d =>
            {
                d(ViewModel!.ShowPunishmentDialog.RegisterHandler(DoShowPunishmentDialogAsync));
                d(ViewModel!.ShowCustomConfigurationWindow.RegisterHandler(DoShowConfigurationDialogAsync));
                d(ViewModel!.ShowEndGameDialog.RegisterHandler(DoShowEndGameDialogAsync));
            });
        }

        private async void OnOpened(object? sender, EventArgs e)
        {
            var ownerWindow = this;
            var window = new ConsentWindow();
            var result = await window.ShowDialog<bool>(ownerWindow);
            if (!result) { Environment.Exit(0); }
        }

        private void RulesButton_OnClick(object? sender, RoutedEventArgs e)
        {
            var ownerWindow = this;
            var window = new RulesWindow();
            window.ShowDialog<object>(ownerWindow);
        }
        
        private async Task DoShowConfigurationDialogAsync(InteractionContext<FieldConfiguratorViewModel, FieldConfiguration?> interaction)
        {
            var dialog = new FieldConfiguratorWindow()
            {
                DataContext = interaction.Input
            };

            var result = await dialog.ShowDialog<FieldConfiguration?>(this);
            interaction.SetOutput(result);
        }
        
        private async Task DoShowPunishmentDialogAsync(InteractionContext<PunishmentViewModel, PictureContainer?> interaction)
        {
            var dialog = new PunishmentWindow()
            {
                DataContext = interaction.Input
            };

            var result = await dialog.ShowDialog<PictureContainer?>(this);
            interaction.SetOutput(result);
        }
        
        private async Task DoShowEndGameDialogAsync(InteractionContext<EndgameViewModel, Unit> interaction)
        {
            var dialog = new EndgameWindow()
            {
                DataContext = interaction.Input
            };

            await dialog.ShowDialog(this);
            interaction.SetOutput(new Unit());
        }
    }
}