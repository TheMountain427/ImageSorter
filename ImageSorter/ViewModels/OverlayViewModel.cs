

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

    public OverlayViewModel(AppState CurrentAppState, ViewModelBase ViewModelToDisplay, ICommand CloseOverlay, bool AllowClickOff) : base(CurrentAppState)
    {
        this.CurrentAppState = CurrentAppState;
        this.CloseOverlay = CloseOverlay;
        this.AllowClickOff = AllowClickOff;

        this.OverlayRouter.Navigate.Execute(ViewModelToDisplay);

        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable
                .Create(() => this.HandleDeactivation())
                .DisposeWith(disposables);

            this.CurrentAppState.IsWorkSpaceOverlayEnabled = true;

        });
    }

    private void HandleDeactivation()
    {
        this.CurrentAppState.IsWorkSpaceOverlayEnabled = false;
    }
}