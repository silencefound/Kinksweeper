using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kinksweeper.Views;

public partial class RulesWindow : Window
{
    public RulesWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}