

using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows.Input;

namespace ImageSorter.ViewModels;


// Overlay VM that will darken the background and pass another view to center screen
public class OverlayViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public override string UrlPathSegment { get; } = "Overlay";

    public ICommand CloseOverlay { get; }

    public RoutingState OverlayRouter { get; } = new RoutingState();

    // If clicking the background should close the overlay, set to false for required stuff
    public bool AllowClickOff { get; } = true;

    public OverlayViewModel(AppState AppState, ViewModelBase ViewModelToDisplay,  ICommand CloseOverlay, bool AllowClickOff)
    {
        this.CurrentAppState = AppState;
        this.CloseOverlay = CloseOverlay;
        this.AllowClickOff = AllowClickOff;

        this.OverlayRouter.Navigate.Execute(ViewModelToDisplay);

        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable
                .Create(() => this.HandleDeactivation())
                .DisposeWith(disposables);

        });
    }

    private void HandleDeactivation() { }
}