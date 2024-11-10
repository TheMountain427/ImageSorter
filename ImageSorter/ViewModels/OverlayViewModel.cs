

using ImageSorter.Models;
using ReactiveUI;
using System.Windows.Input;

namespace ImageSorter.ViewModels;


// Overlay VM that will darken the background and pass another view to center screen
public class OverlayViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "Overlay";

    public ICommand CloseOverlay { get; }

    public RoutingState OverlayRouter { get; } = new RoutingState();

    // If clicking the background should close the overlay, set to false for required stuff
    public bool AllowClickOff { get; } = true;

    public OverlayViewModel(AppState appState, ViewModelBase viewModel,  ICommand closeOverlay, bool allowClickOff)
    {
        this.CurrentAppState = appState;
        this.CloseOverlay = closeOverlay;
        this.AllowClickOff = allowClickOff;

        this.OverlayRouter.Navigate.Execute(viewModel);
    }
}