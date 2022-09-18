using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Kinksweeper.ViewModels;
using ReactiveUI;

namespace Kinksweeper.Views;

public partial class PunishmentWindow : ReactiveWindow<PunishmentViewModel>
{
    
    public PunishmentWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        this.WhenActivated(d => d(ViewModel!.SelectPunishmentCommand.Subscribe(Close)));
        Closing += OnClosing;
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        var vm = (PunishmentViewModel)DataContext!;
        e.Cancel = string.IsNullOrWhiteSpace(vm.SelectedPunishment);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}