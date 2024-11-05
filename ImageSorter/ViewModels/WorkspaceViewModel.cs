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

    public RoutingState WorkspaceControlsRouter { get; } = new RoutingState();

    public List<ImageDetails>? SortedImageDetails { get; protected set; }

    private ImgOrder _imageSortOrder;
    public ImgOrder ImageSortOrder
    {
        get { return _imageSortOrder; }
        protected set { this.RaiseAndSetIfChanged(ref _imageSortOrder, value); }
    }


    private int _currentImageIndex;
    public int CurrentImageIndex
    {
        get { return _currentImageIndex; }
        protected set { this.RaiseAndSetIfChanged(ref _currentImageIndex, value); }
    }
    //public int NextImageIndex { get; protected set; }
    //public int PreviousImageIndex { get; protected set; }

    //public CurrentImageView NextMainImageView { get; protected set; }
    public CurrentImageViewModel NextImageVM { get; protected set; }
    public CurrentImageViewModel CurrentImageVM { get; protected set; }
    public CurrentImageViewModel PreviousImageVM { get; protected set; }


    public void ChangeImageRight()
    {
        // Don't think about it
        if (NextImageVM is not null)
        {
            CurrentImageRouter.Navigate.Execute(NextImageVM);
            PreviousImageVM = CurrentImageVM;
            CurrentImageVM = NextImageVM;
            CurrentImageIndex = SortedImageDetails.IndexOf(CurrentImageVM.ImageDetails);

            if (CurrentImageIndex < SortedImageDetails.Count)
            {
                NextImageVM = new CurrentImageViewModel(SortedImageDetails[CurrentImageVM.CurrentIndex + 1], CurrentImageVM.CurrentIndex + 1);
            }
            else
            {
                NextImageVM = null;
            }
        }
    }

    public void ChangeImageLeft()
    {
        // Don't think about it
        if (PreviousImageVM is not null)
        {
            CurrentImageRouter.Navigate.Execute(PreviousImageVM);
            NextImageVM = CurrentImageVM;
            CurrentImageVM = PreviousImageVM;
            CurrentImageIndex = SortedImageDetails.IndexOf(CurrentImageVM.ImageDetails);

            if (CurrentImageIndex > 0)
            {
                PreviousImageVM = new CurrentImageViewModel(SortedImageDetails[CurrentImageVM.CurrentIndex - 1], CurrentImageVM.CurrentIndex - 1);
            }
            else
            {
                PreviousImageVM = null;
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

    public void FilterImageOne()
    {
        this.ProjectConfig.SetImageFilterValue(CurrentImageVM.ImageDetails, "one");
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
        WorkspaceControlsRouter = new RoutingState();

        GetImageDetailsSorted(ImageSortOrder);

        if (SortedImageDetails is not null)
        {
            CurrentImageVM = new CurrentImageViewModel(SortedImageDetails[CurrentImageIndex], CurrentImageIndex);
            CurrentImageRouter.Navigate.Execute(CurrentImageVM);
            
            if (CurrentImageIndex < SortedImageDetails.Count)
            {
                NextImageVM = new CurrentImageViewModel(SortedImageDetails[CurrentImageVM.CurrentIndex + 1], CurrentImageVM.CurrentIndex + 1);
            }
            else
            {
                NextImageVM = null;
            }

            // For later if loading project in middle of image list
            if (CurrentImageIndex > 0)
            {
                PreviousImageVM = new CurrentImageViewModel(SortedImageDetails[CurrentImageVM.CurrentIndex - 1], CurrentImageVM.CurrentIndex - 1);
            }
            else
            {
                PreviousImageVM = null;
            }
        }

        WorkspaceControlsRouter.Navigate.Execute(new WorkspaceControlsViewModel());
    }
}
