using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace Kinksweeper.Views;

public partial class ConsentWindow : Window
{
    public ConsentWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        this.GetControl<Button>("Agree").Command = ReactiveCommand.Create(() => { Close(true); });
        this.GetControl<Button>("Decline").Command = ReactiveCommand.Create(() => { Close(false); });
        
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}