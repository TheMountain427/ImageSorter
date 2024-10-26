using ReactiveUI;
using System;
using System.Reactive;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class ProjectSelectionViewModel : ReactiveObject, IRoutableViewModel
{
    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = "ProjectSelection";

    public RoutingState MainRouter { get; set; }


    private bool _projectSelected = false;

    public bool ProjectSelected
    {
        get { return _projectSelected; }
        protected set { this.RaiseAndSetIfChanged(ref _projectSelected, value); }
    }

    public void SelectProject()
    {
        ProjectSelected = true;
    }

    

    public ICommand GoToWorkspace { get; }

    private void _goToWorkspace()
    {
        MainRouter.Navigate.Execute(new WorkspaceViewModel(this.HostScreen, this.MainRouter));
    }



    public ProjectSelectionViewModel(IScreen screen, RoutingState router)
    {
        MainRouter = router;
        HostScreen = screen;

        var CanContinue = this.WhenAnyValue(x => x.ProjectSelected);

        GoToWorkspace = ReactiveCommand.Create(_goToWorkspace, CanContinue);




    }

}