using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;
using System.Linq;

namespace ImageSorter.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    // If this is null
    private ZoomBorder? _currentImageZoomBorder { get; set; }

    private RoutedViewHost? _mainRoutedViewHost { get; }

    // Allow clicking the background to exit object focus, Grid must be focusable and transparent
    // Note: this makes the grid always the focus when not in selectable things, like a textbox
    // This might cause weird issues with KeyDown events since some controls may not be able to get focus normally
    public void Background_PointerPressed(object sender, PointerPressedEventArgs eventArgs)
    {
        // Ok hear me out, this isn't as bad is it looks
        // I want to keep the click background function, but I need to be able to select the ZoomBorder
        // to send key events to it. 
        // A worse option is to use App.Toplevel.Keydown then filter those events.
        // But that fires even when typing in a textbox, which would require more hack filters.
        if (sender is Grid grid)
        {
            // If we are at workspace view, and zoomborder has not been found, go find it
            if (_mainRoutedViewHost is not null && _mainRoutedViewHost.Content is WorkspaceView wv && _currentImageZoomBorder is null)
            {
                this._currentImageZoomBorder = wv.FindControl<ZoomBorder>("CurrentImageZoomBorder");
            }

            // If zoomborder is null then we are not at the workspace view, so freely focus background
            if (_currentImageZoomBorder is null)
            {
                grid.Focus();
                // Actually I'm just going to focus the ZoomBorder by default. Makes hotkeys easier
                _currentImageZoomBorder?.Focus();
            }
            // If zoom border is not null, then we are at workspace view 
            // If pointer is over zoomborder, then disable background focus so we can focus that
            else if (!_currentImageZoomBorder.IsPointerOver)
            {
                grid.Focus();
                // Actually I'm just going to focus the ZoomBorder by default. Makes hotkeys easier
                _currentImageZoomBorder?.Focus();
            }
        }

        // Now we can focus the zoomborder and handle KeyDown events on its code behind
    }


    public MainWindow()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);

        // Store the RouterViewHost so we don't have to find it everytime on our checks
        this._mainRoutedViewHost = this.FindControl<RoutedViewHost>("MainRoutedViewHost");


        // Adding F12 DevTools manually since this MainWindow does
        // not inherit from Window
#if DEBUG
        this.AttachDevTools();
#endif
    }

}