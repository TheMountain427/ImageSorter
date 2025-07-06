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

    public static IClassicDesktopStyleApplicationLifetime Desktop { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();

            // Set App.TopLevel here
            TopLevel = TopLevel.GetTopLevel(desktop.MainWindow)!;
            // Changed formated here so that Top Level is loaded before the view models
            // This way we can access StorageProvider on ViewModel load
            // This also lets us access keydown events from anywhere
            desktop.MainWindow.DataContext = new MainWindowViewModel();

            // Allow access to ApplicationLiftetime for App Exit event
            Desktop = desktop;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
