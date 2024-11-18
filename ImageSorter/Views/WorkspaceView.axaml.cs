using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;

namespace ImageSorter.Views;

public partial class WorkspaceView : ReactiveUserControl<WorkspaceViewModel>
{
    // This doesn't work since we aren't using : Window and InitializeComponent()
    // because we are using Reactive and stuff
    // Normally this would auto bind to the xaml control with the same name
    // Have to figure out how to do this with Reactive I guess
    private readonly ZoomBorder? _zoomBorder;

    private void ZoomBorder_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.F:
                _zoomBorder?.Fill();
                break;
            case Key.U:
                _zoomBorder?.Uniform();
                break;
            case Key.R:
                _zoomBorder?.ResetMatrix();
                break;
            case Key.T:
                _zoomBorder?.ToggleStretchMode();
                _zoomBorder?.AutoFit();
                break;
        }
    }

    public WorkspaceView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);

        _zoomBorder = this.Find<ZoomBorder>("ZoomBorder");
        if (_zoomBorder != null)
        {
            _zoomBorder.KeyDown += ZoomBorder_KeyDown;
        }
    }
}
