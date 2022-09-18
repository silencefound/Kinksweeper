using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Kinksweeper.Models;

namespace Kinksweeper.ViewModels;

public class EndgameViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> RestartGameCommand { get; }
    public ReactiveCommand<Unit, Unit> LeftButton { get; }
    public ReactiveCommand<Unit, Unit> RightButton { get; }

    private bool _leftButtonEnabled;

    public bool LeftButtonEnabled
    {
        get => _leftButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _leftButtonEnabled, value);
    }

    private bool _rightButtonEnabled = true;

    public bool RightButtonEnabled
    {
        get => _rightButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _rightButtonEnabled, value);
    }


    private Avalonia.Media.Imaging.Bitmap? _currentPicture;

    public Avalonia.Media.Imaging.Bitmap CurrentPicture
    {
        get => _currentPicture!;
        set => this.RaiseAndSetIfChanged(ref _currentPicture, value);
    }

    private readonly List<PictureContainer> picturesContainerHashSet;

    private string? _debugInfo;

    public string? DebugInfo
    {
        get => _debugInfo;
        set => this.RaiseAndSetIfChanged(ref _debugInfo, value);
    }

    private int _currentIndex;

    private int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (value < 0)
            {
                return;
            }

            if (value >= picturesContainerHashSet.Count)
            {
                return;
            }

            LeftButtonEnabled = value != 0;
            RightButtonEnabled = value != picturesContainerHashSet.Count - 1;

            this.RaiseAndSetIfChanged(ref _currentIndex, value);
        }
    }

    private string _originalPostURL = "";

    public string OriginalPostURL
    {
        get => _originalPostURL;
        set => this.RaiseAndSetIfChanged(ref _originalPostURL, value);
    }

    public bool ProperlyClosed { get; private set; }

    public EndgameViewModel(HashSet<PictureContainer> kinks)
    {
        picturesContainerHashSet = new List<PictureContainer>();
        foreach (var kink in kinks)
        {
            picturesContainerHashSet.Add(kink);
        }

        RestartGameCommand = ReactiveCommand.Create(() => { ProperlyClosed = true; });
        LeftButton = ReactiveCommand.CreateFromTask(async () =>
        {
            CurrentIndex--;
            await DownloadImage();
        });
        RightButton = ReactiveCommand.CreateFromTask(async () =>
        {
            CurrentIndex++;
            await DownloadImage();
        });

        if (picturesContainerHashSet.Count > 0)
        {
            CurrentIndex = 0;
            DebugInfo = "Your selections:";
            Task.Run(async () => await DownloadImage());
            return;
        }


        DebugInfo =
            "You finished the game without a single failure.\n" +
            "Try harder options, because failures are what this game is intended for ^^";
    }

    private async Task DownloadImage()
    {
        try
        {
            DebugInfo = "Downloading the picture...";
            var picContainer = picturesContainerHashSet[CurrentIndex];
            CurrentPicture = await picContainer.GetPicture() ??
                             throw new Exception($"Couldn't download the image {picContainer.PictureURL}");
            OriginalPostURL = "https://rule34.xxx/index.php?page=post&s=view&id=" + picContainer.postID;
            DebugInfo = "Your selections:";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            DebugInfo = ex.ToString();
        }
    }
}