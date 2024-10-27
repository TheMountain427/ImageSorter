using Avalonia.Controls;
using ReactiveUI;
using System.Linq;

namespace ImageSorter.ViewModels;

public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel
{
    public IScreen HostScreen { get; set; }

    public abstract string UrlPathSegment { get; }

    public RoutingState MainRouter { get; set; }

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
}
