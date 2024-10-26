using ReactiveUI;
using System.Linq;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class WorkspaceViewModel : ReactiveObject, IRoutableViewModel
{

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = "WorkspaceView";

    public RoutingState MainRouter { get; set; }

    private bool _isDebug { get; } = true;


    public void Dbg_GoToProjectSelection()
    {
        if (_isDebug)
        {
            _goToProjectSelectionByName();
        }
    }

    private void _goToProjectSelection()
    {
        MainRouter.NavigateAndReset.Execute(new ProjectSelectionViewModel(this.HostScreen, this.MainRouter));
    }

    private void _goToProjectSelectionByName()
    {
        var routableViewModel = MainRouter.NavigationStack.FirstOrDefault(x => x.UrlPathSegment == "ProjectSelection");
        MainRouter.Navigate.Execute(routableViewModel);
    }
    

    private string _greeting = "Welcome to Avalonia!";
    public string Greeting
    {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);

    }

    public void BtnCommand()
    {
        Greeting = "butthole";
    }
    public WorkspaceViewModel(IScreen screen, RoutingState router)
    {
        MainRouter = router;
        HostScreen = screen;

       
        
    }
}
