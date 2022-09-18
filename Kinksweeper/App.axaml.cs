using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Kinksweeper.ViewModels;
using Kinksweeper.Views;

namespace Kinksweeper
{
    public partial class App : Application
    {
        public static MainWindow? MainWindow;
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };

                MainWindow = desktop.MainWindow as MainWindow;

                var x = MainWindow?.GetVisualDescendants();
                
                (desktop.MainWindow.DataContext as MainWindowViewModel)?.InitializeGame();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}