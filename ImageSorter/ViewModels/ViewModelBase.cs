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

    private bool _isDebug = true;
    public bool IsDebug
    {
        get { return _isDebug; }
        set { this.RaiseAndSetIfChanged(ref _isDebug, value); }
    }
        
    private void _goToViewByUrl(string urlPathSegment)
    {
        MainRouter.Navigate.Execute(MainRouter.NavigationStack.FirstOrDefault(x => x.UrlPathSegment == urlPathSegment));
    }

    public ViewModelBase(AppState AppState)
    {
        this.CurrentAppState = AppState;
    }
}
