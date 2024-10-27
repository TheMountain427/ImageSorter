using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using ImageSorter.ViewModels;
using ImageSorter.Views;

namespace ImageSorter;

public partial class App : Application
{
    // Going to allow TopLevel Access from anywhere using 'App.TopLevel'
    // Cause fuck all that other shit, this is the least megamind way to get to StorageProvider
    // https://github.com/AvaloniaUI/Avalonia/discussions/13599#discussioncomment-7562209
    public static TopLevel TopLevel { get; private set; }

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
                DataContext = new MainWindowViewModel()
            };
            // Set App.TopLevel here
            TopLevel = TopLevel.GetTopLevel(desktop.MainWindow);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
