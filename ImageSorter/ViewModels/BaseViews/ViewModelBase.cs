using Avalonia.Controls;
using ImageSorter.Models;
using ReactiveUI;
using System.Linq;

namespace ImageSorter.ViewModels;



public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel
{
    public IScreen HostScreen { get; set; }

    public abstract string? UrlPathSegment { get; }

    public RoutingState MainRouter { get; set; }

    public AppState CurrentAppState { get; set; }
            
    public ViewModelBase(AppState AppState)
    {
        this.CurrentAppState = AppState;
    }
}
