using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kinksweeper.Views;

public partial class MineField : UserControl
{
    public Grid? _MineGrid;
    public MineField()
    {
        InitializeComponent();
        _MineGrid = LogicalChildren.OfType<Grid>().FirstOrDefault();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}