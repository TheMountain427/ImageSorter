using Avalonia.Animation;
using ImageSorter.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using static ImageSorter.Models.Enums;
using static Avalonia.Animation.PageSlide.SlideAxis;
using ImageSorter.Views;
using System;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using static ImageSorter.Models.SortOptionKey;
using System.Threading.Tasks;
using System.Threading;
using System.Reactive;
using System.Reactive.Linq;

namespace ImageSorter.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{

    public override string UrlPathSegment { get; } = "WorkspaceView";

    public ProjectConfig ProjectConfig { get; protected set; }

    public RoutingState CurrentImageRouter { get; } = new RoutingState();

    public RoutingState WorkspaceControlsRouter { get; } = new RoutingState();

    public List<ImageDetails>? SortedImageDetails { get; protected set; }

    public RoutingState WorkspaceFilterRouter { get; } = new RoutingState();

    public RoutingState WorkspaceAlphaReferenceRouter { get; } = new RoutingState();

    public RoutingState WorkspaceBetaReferenceRouter { get; } = new RoutingState();

    public RoutingState OverlayRouter { get; } = new RoutingState();

    public List<ImageDetails> AlphaReferenceImages { get; set; }

    public List<ImageDetails> BetaReferenceImages { get; set; }

    public WorkspaceReferenceImageViewModel AlphaReferenceViewModel { get; }

    public WorkspaceReferenceImageViewModel BetaReferenceViewModel { get; }

    public ICommand OverlaySuccessCommand { get; set; }

    public SortConfigs SortConfigs { get; set; } = new SortConfigs();

    private bool _isSortWarningUp;
    public bool IsSortWarningUp
    {
        get { return _isSortWarningUp; }
        set { _isSortWarningUp = value; _onIsSortWarningUpChange?.Invoke(this, EventArgs.Empty); }
    }
    private EventHandler _onIsSortWarningUpChange;
    public event EventHandler OnIsSortWarningUpChange
    {
        add { _onIsSortWarningUpChange += value; }
        remove { _onIsSortWarningUpChange -= value; }
    }

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

    public void ResetImagePosition()
    {

    }

    public void GetImageDetailsSorted(ImgOrder ImgOrder)
    {
        if (ProjectConfig is not null && ProjectConfig.InputImages is not null)
        {
            var imgDetails = ProjectConfig.InputImages;

            SortedImageDetails = ImgOrder switch
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


    private void ManageReferenceSplit(object sender, EventArgs e)
    {
        // Make alpha have the greater number of refs if the total reference count is odd
        int alphaCount = this.ProjectConfig.ReferenceImages.Count / 2 + (this.ProjectConfig.ReferenceImages.Count % 2);

        this.AlphaReferenceImages = this.ProjectConfig.ReferenceImages.Take(alphaCount).ToList();
        this.BetaReferenceImages = this.ProjectConfig.ReferenceImages.Skip(alphaCount).ToList();

        this.AlphaReferenceViewModel.UpdateReferenceCollection(this.AlphaReferenceImages);
        this.BetaReferenceViewModel.UpdateReferenceCollection(this.BetaReferenceImages);
    }


    public ICommand CloseOverlayView { get; }
    private void _closeOverlayView()
    {
        CurrentAppState.IsWorkSpaceOverlayEnabled = false;

        // Idk which to use, probably clear
        //OverlayRouter.NavigateBack.Execute();
        OverlayRouter.NavigationStack.Clear();
    }

    public void ConfirmSortingOfImages()
    {
        var imageSortFilters = GetSortedImageFilters(this.ProjectConfig.InputImages);

        var referenceFilters = GetSortedImageFilters(this.ProjectConfig.ReferenceImages);


        // Check both and open window after
        var orphanedImageFilters = imageSortFilters.Except(referenceFilters);
        bool filtersContainsUnsorted = imageSortFilters.Contains("Unsorted");

        var sortConfirmations = new List<SortConfirmation>();

        // Create a warning overlay view model requiring interaction

        // Check if any images remain unsorted, create a line item if so
        if (filtersContainsUnsorted)
        {
            var unsortedCount = this.SortedImageDetails.Where(x => x.FilteredValue == "Unsorted").Count();

            var unsortedConfirmationsText = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(this.SortConfigs.GetOptionText(IgnoreUnsorted), "Continue, ignoring any images that are unsorted"),
                new KeyValuePair<string, string>(this.SortConfigs.GetOptionText(IncludeUnsorted), "Continue, including any images that are unsorted")
            };

            var unsortedWarningText = $"Warning: {unsortedCount} unsorted images remain.";

            var unsortedSortConfirmation = new SortConfirmation(WarningText: unsortedWarningText,
                                                                Pairs: unsortedConfirmationsText,
                                                                RequiredToContinue: true);

            sortConfirmations.Add(unsortedSortConfirmation);
        }

        // Check if any images have orphaned filterValues, create a line item if so
        if (orphanedImageFilters.Any())
        {
            var orphanedConfirmationsText = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(this.SortConfigs.GetOptionText(IgnoreOrphans), "Continue, ignoring any images that contian orphaned filters"),
                new KeyValuePair<string, string>(this.SortConfigs.GetOptionText(IncludeOrphans), "Continue, including any images that contian orphaned filters")
            };

            var orphans = new List<string>();
            foreach (var orphan in orphanedImageFilters)
            {
                orphans.Add($"\"{orphan}\"");
            }
            var orphanedWarningText = $"Warning: {orphanedImageFilters.Count()} images contain orphaned filters. These orphans are: \n {string.Join(", ", orphans)}";

            var orphanedSortConfirmation = new SortConfirmation(WarningText: orphanedWarningText,
                                                                Pairs: orphanedConfirmationsText,
                                                                RequiredToContinue: true);

            sortConfirmations.Add(orphanedSortConfirmation);
        }




        // If their are warnings, we need to prompt for how to proceed.
        // Also some ghetto event handling as I would rather the overlay be an async method that awaits values
        // then we would just continue this method, but idk how
        if (sortConfirmations.Any())
        {
            // Create a Action that will be run when the Continue/Confirm button is selected
            Action<IEnumerable<string>> _processOptions = stringOptions =>
            {
                foreach (var option in stringOptions)
                {
                    if (this.SortConfigs.ContainsText(option))
                    {
                        this.SortConfigs.SetValue(option, true);
                    }
                }
                this.IsSortWarningUp = false;
                this.CloseOverlayView.Execute(null);
            };
            var processWarningOptions = ReactiveCommand.Create(_processOptions);

            // Create the command that will be run when the Cancel button is hit
            var cancelCommand = ReactiveCommand.Create(() =>
            {
                // Unsubscribe since we are not going to show the preview after canceling
                this.OnIsSortWarningUpChange -= ShowPreviewAfterWarning;
                this.IsSortWarningUp = false;
                this.CloseOverlayView.Execute(null);
            });

            // Create the warning view
            var sortConfirmationVM = new SortConfirmationViewModel(SortConfirmations: sortConfirmations,
                                                                   CloseOverlay: this.CloseOverlayView,
                                                                   OnSuccessCommand: processWarningOptions,
                                                                   OnCancelCommand: cancelCommand);

            // Mark this as up, we will have a handler that will show preview if continue is selected
            this.IsSortWarningUp = true;

            // Create and navigate to the overlay view
            OverlayRouter.Navigate.Execute(new OverlayViewModel(AppState: CurrentAppState,
                                                                ViewModelToDisplay: sortConfirmationVM,
                                                                CloseOverlay: CloseOverlayView,
                                                                AllowClickOff: false));
            // Subscribe to watch for OnSuccessCommand to run and mark IsSortWarningUp = 
            this.OnIsSortWarningUpChange += ShowPreviewAfterWarning;
        }
        else
        {
            // No warnings, we can proceed to the sort preview
            ShowPreview();
        }

    }

    private void ShowPreviewAfterWarning(object sender, EventArgs e)
    {
        // Show the sort preview
        ShowPreview();
        // Unsubscribe, we only want to be subscribed when the view is up
        this.OnIsSortWarningUpChange -= ShowPreviewAfterWarning;
    }

    private void ShowPreview()
    {

    }

    private void SortImagesIntoGroups(IEnumerable<ImageDetails> ImageDetails, IEnumerable<string> FilterValues)
    {


    }

    private IEnumerable<string> GetSortedImageFilters(IEnumerable<ImageDetails> ImageDetails)
    {
        return ImageDetails.Select(x => x.FilteredValue).Distinct();
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
        CurrentAppState.IsWorkSpaceOverlayEnabled = true;
        OverlayRouter.Navigate.Execute(new OverlayViewModel(this.CurrentAppState, new DebugViewModel(), this.CloseOverlayView, true));
    }



    public WorkspaceViewModel(IScreen screen, RoutingState router, AppState appState, ProjectConfig projectConfig)
    {
        this.MainRouter = router;
        this.HostScreen = screen;
        this.CurrentAppState = appState;
        this.ProjectConfig = projectConfig;
        this.ImageSortOrder = ImgOrder.DescFileName;
        this.CurrentImageIndex = 0;
        this.CurrentImageRouter = new RoutingState();
        this.WorkspaceControlsRouter = new RoutingState();

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

        // Pass through commands to control vm that navigate the main image
        var imageCommands = new ImageCommands()
        {
            NavigateNextMainImage = ReactiveCommand.Create(ChangeImageRight),
            NavigatePreviousMainImage = ReactiveCommand.Create(ChangeImageLeft),
            ResetMainImagePosition = ReactiveCommand.Create(ResetImagePosition)
        };

        WorkspaceControlsRouter.Navigate.Execute(new WorkspaceControlsViewModel(this.ProjectConfig, this.CurrentAppState, imageCommands));

        // Side pane that allows modification of filter amounts
        WorkspaceFilterRouter.Navigate.Execute(new WorkspaceFilterViewModel(this.ProjectConfig, this.CurrentAppState));


        // Time for some magic collections. Need to split the reference collection to both sides
        // but allow changes to properties on the collection to reflect the original list
        // Oh that actually wasn't bad

        // Oh this will break on a new project cause no filters
        // Well actually maybe not but just to be sure
        // Also force 2 cause otherwise you wouldn't be sorting anything
        if (this.ProjectConfig.ReferenceImages.Count <= 1)
        {
            while (this.ProjectConfig.ReferenceImages.Count <= 1)
            {
                this.ProjectConfig.ReferenceImages.Add(new ImageDetails());
            }
        }

        // Make alpha have the greater number of refs if the total reference count is odd
        int alphaCount = this.ProjectConfig.ReferenceImages.Count / 2 + (this.ProjectConfig.ReferenceImages.Count % 2);

        this.AlphaReferenceImages = this.ProjectConfig.ReferenceImages.Take(alphaCount).ToList();
        this.BetaReferenceImages = this.ProjectConfig.ReferenceImages.Skip(alphaCount).ToList();

        // Have to save the view models cause I need to notify the VM's that the collection changes
        // Other option is to just make a new VM every time..? Seems smelly to do that
        this.AlphaReferenceViewModel = new WorkspaceReferenceImageViewModel(this.CurrentAppState, this.ProjectConfig, AlphaReferenceImages, ReferenceViewIdentifier.Alpha);
        this.BetaReferenceViewModel = new WorkspaceReferenceImageViewModel(this.CurrentAppState, this.ProjectConfig, BetaReferenceImages, ReferenceViewIdentifier.Beta);

        WorkspaceAlphaReferenceRouter.Navigate.Execute(this.AlphaReferenceViewModel);
        WorkspaceBetaReferenceRouter.Navigate.Execute(this.BetaReferenceViewModel);

        // Handle rebuilding the image split when reference image count changes
        this.ProjectConfig.ReferenceImages.CollectionChanged += ManageReferenceSplit;

        this.CloseOverlayView = ReactiveCommand.Create(_closeOverlayView);
    }
}
