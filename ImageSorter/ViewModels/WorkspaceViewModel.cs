using ImageSorter.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using static ImageSorter.Models.Enums;

namespace ImageSorter.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{

    public override string UrlPathSegment { get; } = "WorkspaceView";

    public ProjectConfig ProjectConfig { get; protected set; }

    public RoutingState CurrentImageRouter { get; } = new RoutingState();

    public List<ImageDetails> SortedImageDetails { get; protected set; }

    public ImgOrder ImageSortOrder { get; set; }

    public int CurrentImageIndex { get; set; }

    public void ChangeImageRight()
    {
        if (CurrentImageIndex < SortedImageDetails.Count)
        {
            if (CurrentImageIndex < SortedImageDetails.Count - 1)
            {
                CurrentImageIndex++;
            }
            CurrentImageRouter.NavigateAndReset.Execute(new CurrentImageViewModel(SortedImageDetails[CurrentImageIndex], CurrentImageIndex));
        }
    }

    public void ChangeImageLeft()
    {
        if (CurrentImageIndex >= 0)
        {
            if (CurrentImageIndex != 0)
            {
                CurrentImageIndex--;
            }
            CurrentImageRouter.Navigate.Execute(new CurrentImageViewModel(SortedImageDetails[CurrentImageIndex], CurrentImageIndex));
        }

    }

    public void GetImageDetailsSorted(ImgOrder imgOrder)
    {
        var imgDetails = ProjectConfig.InputImages;

        SortedImageDetails = imgOrder switch
        {
            ImgOrder.AscFileName => imgDetails.SortByFileNameAscending().ToList(),
            ImgOrder.DescFileName => imgDetails.SortByFileNameDescending().ToList(),
            ImgOrder.AscFileSize => imgDetails.SortByFileSizeAscending().ThenBy(x => x.FileName).ToList(),
            ImgOrder.DescFileSize => imgDetails.SortByFileSizeDescending().ThenBy(x => x.FileName).ToList(),
            ImgOrder.AscFileCreatedTime => imgDetails.SortByFileCreationTimeAscending().ThenBy(x => x.FileName).ToList(),
            ImgOrder.DescFileCreatedTime => imgDetails.SortByFileCreationTimeDescending().ThenBy(x => x.FileName).ToList(),
            ImgOrder.AscLastModifiedTime => imgDetails.SortByLastModifiedTimeAscending().ThenBy(x => x.FileName).ToList(),
            ImgOrder.DescLastModifiedTime => imgDetails.SortByLastModifiedTimeDescending().ThenBy(x => x.FileName).ToList(),
            _ => imgDetails.ToList()
        };


    }

    // **** Debug **** //
    public void Dbg_GoToProjectSelection()
    {
        if (IsDebug)
        {
            _goToProjectSelectionByName();
        }
    }

    private void _goToProjectSelection()
    {
        MainRouter.NavigateAndReset.Execute(new ProjectSelectionViewModel(this.HostScreen, this.MainRouter, this.CurrentAppState));
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


    public WorkspaceViewModel(IScreen screen, RoutingState router, AppState appState, ProjectConfig projectConfig)
    {
        MainRouter = router;
        HostScreen = screen;
        CurrentAppState = appState;
        ProjectConfig = projectConfig;
        ImageSortOrder = ImgOrder.DescFileName;
        CurrentImageIndex = 0;
        CurrentImageRouter = new RoutingState();

        GetImageDetailsSorted(ImageSortOrder);

        if (SortedImageDetails is not null)
        {
            CurrentImageRouter.Navigate.Execute(new CurrentImageViewModel(SortedImageDetails[CurrentImageIndex], CurrentImageIndex));
        }


    }
}
