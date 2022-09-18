using System;
using System.Reactive;
using Avalonia.Threading;
using Kinksweeper.Models;
using ReactiveUI;

namespace Kinksweeper.ViewModels;

public class FieldConfiguratorViewModel : ViewModelBase
{
    private int _dimension = Constants.DefaultFieldDimension;

    public string Dimension
    {
        get => _dimension.ToString();
        set
        {
            var parsed = int.TryParse(value, out var result);
            if (!parsed || result is <= 0 or > 30)
            {
                result = 30;
            }

            DispatcherTimer.RunOnce(() => this.RaiseAndSetIfChanged(ref _dimension, result),
                TimeSpan.FromMilliseconds(50));

            if (_minesCount > result * result)
            {
                this.RaiseAndSetIfChanged(ref _minesCount, (int)(Constants.DefaultMineDensity * _dimension * _dimension));
            }
        }
    }


    private int _minesCount = (int)(Constants.DefaultMineDensity * Math.Pow(Constants.DefaultFieldDimension, 2));

    public string MinesCount
    {
        get => _minesCount.ToString();
        set
        {
            var parsed = int.TryParse(value, out var result);
            if (!parsed || result <= 0 || result > _dimension * _dimension)
            {
                result = (int)(Constants.DefaultMineDensity * _dimension * _dimension);
            }

            this.RaiseAndSetIfChanged(ref _minesCount, result);
        }
    }

    private int _picsPerPunishment = Constants.DefaultPicsPerPunishment;

    public string PicsPerPunishment
    {
        get => _picsPerPunishment.ToString();
        set
        {
            var parsed = int.TryParse(value, out var result);
            if (!parsed || result is <= 0 or > 50)
            {
                result = Constants.DefaultPicsPerPunishment;
            }

            this.RaiseAndSetIfChanged(ref _picsPerPunishment, result);
        }
    }
    
    public ReactiveCommand<Unit, FieldConfiguration> SelectConfiguration { get; }

    public FieldConfiguratorViewModel()
    {
        SelectConfiguration = ReactiveCommand.Create(() => new FieldConfiguration()
        {
            Dimension = _dimension,
            MinesCount = _minesCount,
            PicsPerPunishment = _picsPerPunishment
        });
    }
}