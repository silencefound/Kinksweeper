using System;
using System.ComponentModel;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Kinksweeper.ViewModels;
using ReactiveUI;

namespace Kinksweeper.Views;

public partial class EndgameWindow : ReactiveWindow<EndgameViewModel>
{
    public EndgameWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        this.WhenActivated(d => d(ViewModel!.RestartGameCommand.Subscribe(_ => Close())));
        Closing += OnClosing;
    }
    
    private void OnClosing(object? sender, CancelEventArgs e)
    {
        var vm = (EndgameViewModel)DataContext!;
        e.Cancel = !vm.ProperlyClosed;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}