using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Kinksweeper.Models;
using ReactiveUI;

namespace Kinksweeper.ViewModels;

public class PunishmentViewModel : ViewModelBase
{
    private readonly List<PictureContainer> pictureContainerList = new();

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

    public string PunishmentTopic { get; set; }

    public string? SelectedPunishment => _selectedPunishment?.PictureURL;
    private PictureContainer? _selectedPunishment;

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

            if (value >= pictureContainerList.Count)
            {
                return;
            }

            LeftButtonEnabled = value != 0;
            RightButtonEnabled = value != pictureContainerList.Count - 1;

            this.RaiseAndSetIfChanged(ref _currentIndex, value);
        }
    }

    private string _originalPostURL = "";

    public string OriginalPostURL
    {
        get => _originalPostURL;
        set => this.RaiseAndSetIfChanged(ref _originalPostURL, value);
    }

    private Avalonia.Media.Imaging.Bitmap? _punishmentPicture;

    public Avalonia.Media.Imaging.Bitmap PunishmentPicture
    {
        get => _punishmentPicture!;
        set => this.RaiseAndSetIfChanged(ref _punishmentPicture, value);
    }

    private async Task DownloadImage()
    {
        try
        {
            if (pictureContainerList.Count == 0)
            {
                return;
            }

            DebugInfo = "Downloading the picture...";
            _selectedPunishment = pictureContainerList[CurrentIndex];
            PunishmentPicture = await _selectedPunishment.GetPicture() ??
                                throw new Exception($"Couldn't download the image {_selectedPunishment.PictureURL}");
            OriginalPostURL = "https://rule34.xxx/index.php?page=post&s=view&id=" + _selectedPunishment.postID;
            DebugInfo = "";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            DebugInfo = ex.ToString();
        }
    }

    public ReactiveCommand<Unit, PictureContainer> SelectPunishmentCommand { get; }
    public ReactiveCommand<Unit, Unit> LeftPunishmentButton { get; }
    public ReactiveCommand<Unit, Unit> RightPunishmentButton { get; }

    public PunishmentViewModel(string kink, int imageCount)
    {
        PunishmentTopic = kink;
        SelectPunishmentCommand = ReactiveCommand.Create(() => _selectedPunishment!);

        LeftPunishmentButton = ReactiveCommand.CreateFromTask(async () =>
        {
            CurrentIndex--;
            await DownloadImage();
        });
        RightPunishmentButton = ReactiveCommand.CreateFromTask(async () =>
        {
            CurrentIndex++;
            await DownloadImage();
        });

        Task.Run(async () =>
        {
            DebugInfo = "Loading a list of pictures...";
            try
            {
                var picturesList = await Rule34ImageProvider.GetKinkPictures(imageCount, new List<string> { kink });
                picturesList.ForEach(s => pictureContainerList.Add(new PictureContainer(s.file_url, s.id)));
                CurrentIndex = 0;
            }
            catch (Exception ex)
            {
                DebugInfo = ex.ToString();
                _selectedPunishment = new PictureContainer("https://xkcd.com/404", 404);
                return;
            }

            await DownloadImage();
        });
    }
}