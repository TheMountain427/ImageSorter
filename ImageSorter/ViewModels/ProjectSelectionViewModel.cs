using ReactiveUI;
using System;
using System.Reactive;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class ProjectSelectionViewModel : ReactiveObject, IRoutableViewModel
{
    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = "ProjectSelection";

    public RoutingState Router { get; set; }

    public void SelectProject()
    {
        ProjectSelected = true;
    }

    private bool _projectSelected = false;

    public bool ProjectSelected
    {
        get { return _projectSelected; }
        protected set { this.RaiseAndSetIfChanged(ref _projectSelected, value); }
    }

    private void GoToMain()
    {
        Router.NavigateAndReset.Execute(new MainViewModel(this.HostScreen));
    }

    public ICommand GoNext { get; }

    public ProjectSelectionViewModel(IScreen screen, RoutingState router)
    {
        Router = router;
        HostScreen = screen;

        var CanContinue = this.WhenAnyValue(x => x.ProjectSelected);

        GoNext = ReactiveCommand.Create(GoToMain, CanContinue);

    }

}