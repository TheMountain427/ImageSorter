using Avalonia.Animation;
using ImageSorter.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using static ImageSorter.Models.Enums;
using static Avalonia.Animation.PageSlide.SlideAxis;
using ImageSorter.Views;

namespace ImageSorter.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{

    public override string UrlPathSegment { get; } = "WorkspaceView";

    public ProjectConfig ProjectConfig { get; protected set; }

    public RoutingState CurrentImageRouter { get; } = new RoutingState();

    public List<ImageDetails>? SortedImageDetails { get; protected set; }

    public ImgOrder ImageSortOrder { get; set; }


    public int CurrentImageIndex { get; protected set; }
    public int NextImageIndex { get; protected set; }
    public int PreviousImageIndex { get; protected set; }

    //public CurrentImageView NextMainImageView { get; protected set; }
    public CurrentImageViewModel NextMainImageVM { get; protected set; }
    public CurrentImageViewModel CurrentImageVM { get; protected set; }
    public CurrentImageViewModel PreviousImageVM { get; protected set; }


    public void ChangeImageRight()
    {
        // wtf am I doing just make this a list of CurrentImageViewModels
        if (SortedImageDetails is not null && CurrentImageIndex < SortedImageDetails.Count - 1)
        {
            CurrentImageIndex++;
            CurrentImageRouter.Navigate.Execute(NextMainImageVM);
            CurrentImageVM = NextMainImageVM;

            if (NextImageIndex < SortedImageDetails.Count - 1)
            {
                PreviousImageVM = CurrentImageVM;
                NextMainImageVM = new CurrentImageViewModel(SortedImageDetails[NextImageIndex], NextImageIndex);
                NextImageIndex++;
                PreviousImageIndex++;
            }
        }
    }

    public void ChangeImageLeft()
    {
        if (SortedImageDetails is not null && CurrentImageIndex > 0)
        {
            CurrentImageIndex--;
            CurrentImageRouter.Navigate.Execute(PreviousImageVM);
            if (PreviousImageIndex > 0)
            {
                NextMainImageVM = CurrentImageVM;
                CurrentImageVM = PreviousImageVM;
                PreviousImageVM = new CurrentImageViewModel(SortedImageDetails[PreviousImageIndex], PreviousImageIndex);
                NextImageIndex--;
                PreviousImageIndex--;
            }
        }

    }

    public void GetImageDetailsSorted(ImgOrder imgOrder)
    {
        if (ProjectConfig is not null && ProjectConfig.InputImages is not null)
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
        PreviousImageIndex = -1;
        CurrentImageIndex = 0;
        NextImageIndex = 1;
        CurrentImageRouter = new RoutingState();

        GetImageDetailsSorted(ImageSortOrder);

        if (SortedImageDetails is not null)
        {
            CurrentImageVM = new CurrentImageViewModel(SortedImageDetails[CurrentImageIndex], CurrentImageIndex);
            CurrentImageRouter.Navigate.Execute(CurrentImageVM);

            if (CurrentImageIndex < SortedImageDetails.Count)
            {
                NextMainImageVM = new CurrentImageViewModel(SortedImageDetails[NextImageIndex], NextImageIndex);
            }

            // For later if loading project in middle of image list
            if (CurrentImageIndex > 0)
            {
                PreviousImageVM = new CurrentImageViewModel(SortedImageDetails[PreviousImageIndex], PreviousImageIndex);
            }
        }
    }
}
