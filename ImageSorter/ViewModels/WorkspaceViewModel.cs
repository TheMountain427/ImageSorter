using ReactiveUI;
using System.Linq;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{

    public override string UrlPathSegment { get; } = "WorkspaceView";


    public void Dbg_GoToProjectSelection()
    {
        if (IsDebug)
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
