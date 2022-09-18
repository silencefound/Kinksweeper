using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Kinksweeper.ViewModels;
using ReactiveUI;

namespace Kinksweeper.Views;

public partial class FieldConfiguratorWindow : ReactiveWindow<FieldConfiguratorViewModel>
{
    public FieldConfiguratorWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        this.WhenActivated(d => d(ViewModel!.SelectConfiguration.Subscribe(Close)));
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}