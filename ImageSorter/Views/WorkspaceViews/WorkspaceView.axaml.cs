using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;
using System.Diagnostics;

namespace ImageSorter.Views;

public partial class WorkspaceView : ReactiveUserControl<WorkspaceViewModel>
{
    private readonly ZoomBorder? _zoomBorder;

    private void ZoomBorder_ZoomChanged(object sender, ZoomChangedEventArgs e)
    {
        Debug.WriteLine($"[ZoomChanged] {e.ZoomX} {e.ZoomY} {e.OffsetX} {e.OffsetY}");
    }

    private void AttachZoomBorderToVM(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (_zoomBorder is not null && _zoomBorder.DataContext is WorkspaceViewModel vm)
        {
            vm.SetZoomBorder(_zoomBorder);
            _zoomBorder.AttachedToLogicalTree -= AttachZoomBorderToVM;

            if (vm.CurrentAppState.DebugMode)
            {
                _zoomBorder.ZoomChanged += ZoomBorder_ZoomChanged;
            }
        }
    }

    public WorkspaceView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);

        // Requires a bit of a hack on MainWindow code behind
        // This is because I have made it so background clicks exit focus of things
        // That, however, makes it so this ZoomBorder cannot be focused
        _zoomBorder = this.Find<ZoomBorder>("CurrentImageZoomBorder");

        if (_zoomBorder is not null)
        {
            //_zoomBorder.PointerEntered += FocusCurrentImageZoomBorder;

            _zoomBorder.AttachedToLogicalTree += AttachZoomBorderToVM;
        }
    }
}
