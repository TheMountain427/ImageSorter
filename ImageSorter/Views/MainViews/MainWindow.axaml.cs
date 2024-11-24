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

    // Set the height of the system bar override
    // It is always 30 on my machine but this a quick attempt to try to
    // handle another system where the value could be different
    // Altered from https://stackoverflow.com/a/71918303 to handle the background grid focus above
    // Surprise, the size changes when maximized lol
    private void SetUpStatusBar()
    {
        if (this.FrameSize is null)
            throw new UnhandledErrorException("Blowing up now");
                
        var titleBarGrid = this.FindControl<Grid>("TitleBar_Grid");

        if (titleBarGrid is null)
            throw new UnhandledErrorException("Kaboom");

        // X,Y of the window including top system bar
        var frameSize = this.FrameSize;
        // X,Y of the window minus the top system bar
        var clientSize = this.ClientSize;

        // System buttons are FrameSizeY - this. 
        // This is what it is on my machine, might change based on scaling or somthing
        double extraPaddingY = 9;

        var statusBarHeight = frameSize.Value.Height - clientSize.Height - extraPaddingY;

        titleBarGrid.Height = statusBarHeight;
        titleBarGrid.MaxHeight = statusBarHeight;

    }

    public MainWindow()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);

        // Store the RouterViewHost so we don't have to find it everytime on our checks
        // What those checks are, I don't remember
        this._mainRoutedViewHost = this.FindControl<RoutedViewHost>("MainRoutedViewHost");

        // Setup the system title bar height
        SetUpStatusBar();

        // Adding F12 DevTools manually since this MainWindow does
        // not inherit from Window
#if DEBUG
        this.AttachDevTools();
#endif
    }

}