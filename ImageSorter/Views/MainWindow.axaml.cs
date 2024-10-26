using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;

namespace ImageSorter.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel> 
{
    public MainWindow()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);


        // Adding F12 DevTools manually since this MainWindow does
        // not inherit from Window
        #if DEBUG
            this.AttachDevTools();
        #endif
    }
}