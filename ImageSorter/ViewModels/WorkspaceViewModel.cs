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
using System.Diagnostics;
using static ImageSorter.Models.Helpers;
using Avalonia.Media.Imaging;
using System.IO;

namespace ImageSorter.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{

    public override string UrlPathSegment { get; } = "WorkspaceView";

    public ProjectConfig ProjectConfig { get; protected set; }

    public RoutingState CurrentImageRouter { get; } = new RoutingState();

    public RoutingState WorkspaceControlsRouter { get; } = new RoutingState();

    public List<ImageDetails> SortedImageDetails { get; protected set; }

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

    private SortEngine SortEngine { get; }

    public ICommand SetImageFilteredValue { get; }

    public ICommand ProgressIncrement { get; }

    private double _sortProgress;
    public double SortProgress
    {
        get { return _sortProgress; }
        set { this.RaiseAndSetIfChanged(ref _sortProgress, value); }
    }

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

    private ImgOrderOption _imageSortOrder;
    public ImgOrderOption ImageSortOrder
    {
        get { return _imageSortOrder; }
        protected set
        {
            this.RaiseAndSetIfChanged(ref _imageSortOrder, value);
            this.ProjectConfig.ImageSortOrder = value.OptionEnum;
        }
    }

    public ImgOrderOptions ImageOrderOptions { get; } = new ImgOrderOptions();

    private int _currentImageIndex;
    public int CurrentImageIndex
    {
        get { return _currentImageIndex; }
        protected set
        {
            // This is smelly but whatever. 
            // this.ProjectConfig.CurrentImageIndex wasn't updating probably cause of 
            // observables or something so just gunna force it.
            this.RaiseAndSetIfChanged(ref _currentImageIndex, value);
            this.ProjectConfig.CurrentImageIndex = value;
        }
    }
    //public int NextImageIndex { get; protected set; }
    //public int PreviousImageIndex { get; protected set; }

    //public CurrentImageView NextMainImageView { get; protected set; }
    public CurrentImageViewModel NextImageVM { get; protected set; }
    public CurrentImageViewModel CurrentImageVM { get; protected set; }
    public CurrentImageViewModel PreviousImageVM { get; protected set; }

    private void UpdateImageSortOrder(ImgOrderOption ImgOrderOption)
    {
        // Prevent this from running on workspace load
        if (this.CurrentImageVM is not null)
        {
            // Sort order has changed
            var newSortedImageDetails = GetImageDetailsSorted(ImgOrderOption.OptionEnum);

            var newCurrentImageDetailsIndex = newSortedImageDetails.IndexOf(this.CurrentImageVM.ImageDetails);

            if (newSortedImageDetails[newCurrentImageDetailsIndex] is not null)
            {
                this.SortedImageDetails = newSortedImageDetails;

                // Going to use the NextImageVM to load the new CurrentImage spot and then alter the current index.
                // This will prevent flashing the image, then we "ChangeImageRight" which will update everything to the correct spots
                // This keeps the same image on screen, but its position in the sort could be different
                this.NextImageVM?.Dispose();

                this.NextImageVM = new CurrentImageViewModel(SortedImageDetails[newCurrentImageDetailsIndex], newCurrentImageDetailsIndex);
                this.CurrentImageIndex = newCurrentImageDetailsIndex - 1;
                ChangeToNextImage(ImageChangeParam.Single);
            }
        }
    }

    // Don't think about it
    public void ChangeToNextImage(ImageChangeParam param)
    {
        if (this.NextImageVM is not null && SortedImageDetails is not null && SortedImageDetails.Count > 1)
        {
            // If param last image
            if (param == ImageChangeParam.End && CurrentImageIndex != SortedImageDetails.Count - 1)
            {
                this.NextImageVM?.Dispose();
                this.NextImageVM = new CurrentImageViewModel(SortedImageDetails[SortedImageDetails.Count - 1], SortedImageDetails.Count - 1);
            }

            this.CurrentImageRouter.Navigate.Execute(NextImageVM);

            // If PreviousImageVM is not null, Dispose it since it will be out of the "cache"
            this.PreviousImageVM?.Dispose();
            this.PreviousImageVM = CurrentImageVM;

            // If param last image
            if (param == ImageChangeParam.End)
            {
                this.PreviousImageVM?.Dispose();
                this.PreviousImageVM = new CurrentImageViewModel(SortedImageDetails[SortedImageDetails.Count - 2], SortedImageDetails.Count - 2);
            }

            this.CurrentImageVM = NextImageVM;

            this.CurrentImageIndex = SortedImageDetails.IndexOf(CurrentImageVM.ImageDetails);

            if (CurrentImageIndex < SortedImageDetails.Count - 1)
            {
                this.NextImageVM = new CurrentImageViewModel(SortedImageDetails[CurrentImageVM.CurrentIndex + 1], CurrentImageVM.CurrentIndex + 1);
            }
            else
            {
                this.NextImageVM = null;
            }
        }
    }

    public void ChangeToPreviousImage(ImageChangeParam param)
    {
        // Don't think about it
        if (this.PreviousImageVM is not null && SortedImageDetails is not null && SortedImageDetails.Count > 1)
        {
            // If param first image
            if (param == ImageChangeParam.End && CurrentImageIndex != 0)
            {
                this.PreviousImageVM?.Dispose();
                this.PreviousImageVM = new CurrentImageViewModel(SortedImageDetails[0], 0);
            }

            this.CurrentImageRouter.Navigate.Execute(PreviousImageVM);

            // If NextImageVM is not null, Dispose it since it will be out of the "cache"
            this.NextImageVM?.Dispose();
            this.NextImageVM = CurrentImageVM;

            // If param first image
            if (param == ImageChangeParam.End)
            {
                this.NextImageVM?.Dispose();
                this.NextImageVM = new CurrentImageViewModel(SortedImageDetails[1], 1);
            }

            this.CurrentImageVM = PreviousImageVM;
            this.CurrentImageIndex = SortedImageDetails.IndexOf(CurrentImageVM.ImageDetails);

            if (this.CurrentImageIndex > 0)
            {
                this.PreviousImageVM = new CurrentImageViewModel(SortedImageDetails[CurrentImageVM.CurrentIndex - 1], CurrentImageVM.CurrentIndex - 1);
            }
            else
            {
                this.PreviousImageVM = null;
            }
        }

    }

    public void ResetImagePosition()
    {

    }

    private List<ImageDetails> GetImageDetailsSorted(ImgOrder ImgOrder)
    {
        if (ProjectConfig is not null && ProjectConfig.InputImages is not null)
        {
            var imgDetails = ProjectConfig.InputImages;

            var sortedImageDetails = ImgOrder switch
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

            return sortedImageDetails;
        }

        throw new ArgumentNullException("ProjectConfig or InputImages are null");
    }

    private void _setImageFilterValue(string FilterValue)
    {
        // Not sure why I did it like this
        this.ProjectConfig.SetImageFilterValue(CurrentImageVM.ImageDetails, FilterValue);
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
    public ICommand CloseProgressOverlayView { get; }
    private void _closeProgressOverlayView()
    {
        _closeOverlayView();
        // Gotta reset SortProgress after a sort, progress view will always think it's done
        SortProgress = 0;
    }

    public void CheckForImageSortIssues()
    {
        var imageSortFilters = GetSortedImageFilters(this.ProjectConfig.InputImages);

        var referenceFilters = GetSortedImageFilters(this.ProjectConfig.ReferenceImages);


        // Check both and open window after
        var orphanedImageFilters = imageSortFilters.Except(referenceFilters);
        bool filtersContainsUnsorted = imageSortFilters.Contains("Unsorted");

        // Take on unsorted from orphans as it is handled by its own confirmation
        if (filtersContainsUnsorted && orphanedImageFilters.Contains("Unsorted"))
        {
            orphanedImageFilters.ToList().Remove("Unsorted");
        }


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
                // Event handler will handle navigate to the SortPreviewView
                this.IsSortWarningUp = false;
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
        var successCommand = ReactiveCommand.Create(() =>
        {
            BeginSortProcess();
        });

        var cancelCommand = ReactiveCommand.Create(() =>
        {
            this.CloseOverlayView.Execute(null);
        });

        var sortPreviewVM = new SortPreviewViewModel(AppState: this.CurrentAppState,
                                                     ProjectConfig: this.ProjectConfig,
                                                     SortedImageDetails: SortedImageDetails,
                                                     OnSuccessCommand: successCommand,
                                                     OnCancelCommand: cancelCommand);

        OverlayRouter.Navigate.Execute(new OverlayViewModel(AppState: CurrentAppState,
                                                            ViewModelToDisplay: sortPreviewVM,
                                                            CloseOverlay: CloseOverlayView,
                                                            AllowClickOff: false));

    }

    private void BeginSortProcess()
    {
        double imageCount = this.ProjectConfig.InputImages.Count;
        this.SortProgress = 0;
        var inProgessText = "Watch it! I'm trying to work here!";

        double maxProgress = this.SortEngine.CalculateSortProgressMaximum(this.ProjectConfig);

        var progressVM = new InProgressViewModel(MinimumValue: 0,
                                                MaximumValue: maxProgress,
                                                ValueToWatch: this.WhenAnyValue(x => x.SortProgress), // pass an IObservable<double>, so it can be monitored
                                                InProgressText: inProgessText,
                                                PauseOnCompletion: true,
                                                OnSuccessCommand: this.CloseProgressOverlayView,
                                                CompletionMessage: "I'm done!");

        OverlayRouter.Navigate.Execute(new OverlayViewModel(AppState: CurrentAppState,
                                                            ViewModelToDisplay: progressVM,
                                                            CloseOverlay: this.CloseProgressOverlayView,
                                                            AllowClickOff: false));
        // Here we need to come up with a semi fake sort progess maximum.
        // Because I made the progress view and damnit I'm going to use it
        // this.ProjectConfig.InputImages.Count  ->  easy, worth 1 point since we will loop to move them
        // SortImageDetailsForOutput             ->  funky, it has 8 steps so lets make it 10 points
        //                              Huh thats it
        // Yeah, SortImageDetailsForOutput is lightning fast
        // Okay.. we have to make directories for the output so theres some increments there
        // Except I don't know the number until after we start. Changing the progress after we start seems rude
        // Max filters I expect would be 8 though
        // So creating directories               -> worth 8 points


        // Run the image sort as an async command so we can change the UI while it is running
        // We will monitor it with a progress bar
        // Tbh it will probably complete so fast it doesn't really matter
        // It actually is still noticable when image count ~195 so I will leave it

        SortImageDetailsAsyncCommand.Execute((this.SortProgress, imageCount));

        // This is a hacked way to get a command to run async. Just a check to see if the increment is increasing
        Task.Run(async () =>
        {
            await Task.Run(() =>
            {
                while (this.SortProgress < imageCount)
                {
                    Thread.Sleep(500);
                    Debug.WriteLine($"{this.SortProgress}");
                }
            });
        });

    }

    // ????? tuple shit ????? and it works ?????
    // This is the base for the Async command that will sort the images in the background
    // Has no need to be a tuple, in fact it is useless) but I am leaving it cause it's cool
    // The tuple allows more than one variable to be passed in
    public ReactiveCommand<(double Counter, double ImageCount), Unit> SortImageDetailsAsyncCommand { get; }
    // This is the method to be called by SortImageDetailsAsyncCommand
    // https://stackoverflow.com/questions/76400048/ui-freezing-while-executing-async-command-avaloniaui-reactiveui
    private IObservable<Unit> SortImageDetails(double Counter, double ImageCount)
    {
        return Observable.Start(() =>
        {
            // Seperate the images into groups based on filters
            var sortedImageGroups = this.SortEngine.SortImageDetailsForOutput(this.SortedImageDetails, this.ProjectConfig.ReferenceImages, this.SortConfigs);

            // Get the filters that are actually being used since I didn't think far enough ahead 
            var outputNames = this.SortEngine.ConvertSortedGroupsToFilterOutputDirectories(sortedImageGroups);

            // Create the output directories the images will go into
            var outputDirectories = this.SortEngine.CreateOutputDirectories(outputNames, this.ProjectConfig.OutputDirectoryPath.First());

            this.SortEngine.SortImagesIntoDirectories(sortedImageGroups, outputDirectories);



            // Just a test to check if InProgressVM progress bar is working correctly
            //while (Counter < ImageCount)
            //{
            //    Thread.Sleep(100);
            //    Counter++;
            //    this.SortProgress++;
            //    //Debug.WriteLine("Counter increased");
            //}
        });
    }

    public void LoadThumbnails(List<ImageDetails> ImageDetails)
    {
        Task.Run(async () =>
            {
                await Task.Run(async () =>
                {
                    foreach (var detail in ImageDetails)
                    {
                        if (!string.IsNullOrEmpty(detail.FilePath))
                        {
                            await detail.LoadThumbnailAsync();
                        }
                    }
                });
            });
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

    public ICommand DebugCommand;
    public void BtnCommand()
    {
    }



    public WorkspaceViewModel(IScreen screen, RoutingState router, AppState appState, ProjectConfig projectConfig)
    {
        this.MainRouter = router;
        this.HostScreen = screen;
        this.CurrentAppState = appState;
        this.ProjectConfig = projectConfig;
        this.CurrentImageIndex = this.ProjectConfig.CurrentImageIndex;
        this.CurrentImageRouter = new RoutingState();
        this.WorkspaceControlsRouter = new RoutingState();

        // Create the sort engine
        // Pass on the ProgressIncrement for now
        ProgressIncrement = ReactiveCommand.Create(() => this.SortProgress++);
        this.SortEngine = new SortEngine(this.ProgressIncrement);

        // Try to load the image sort order from PorjectConfig, default to DescFileName
        if (this.ImageOrderOptions.TryGetOrderOption(this.ProjectConfig.ImageSortOrder, out ImgOrderOption configOption))
        {
            this.ImageSortOrder = configOption;
        }
        else
        {
            this.ImageSortOrder = this.ImageOrderOptions.GetOrderOption(ImgOrder.DescFileName);
        }


        this.SortedImageDetails = GetImageDetailsSorted(ImageSortOrder.OptionEnum);

        this.WhenAnyValue(x => x.ImageSortOrder).Subscribe(_ => UpdateImageSortOrder(_));

        if (SortedImageDetails is not null)
        {
            CurrentImageVM = new CurrentImageViewModel(SortedImageDetails[CurrentImageIndex], CurrentImageIndex);
            CurrentImageRouter.Navigate.Execute(CurrentImageVM);

            if (CurrentImageIndex < SortedImageDetails.Count - 1)
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

        if (SortedImageDetails is not null)
        {
            LoadThumbnails(this.SortedImageDetails);
        }


        SetImageFilteredValue = ReactiveCommand.Create<string>(_ => _setImageFilterValue(_));
        // Pass through commands to control vm that navigate the main image
        var imageCommands = new ImageCommands()
        {
            // Convert 0 and 1 CommandParameter binding to int > ImageChangeParam
            NavigateNextMainImage = ReactiveCommand.Create<string>(_ => ChangeToNextImage((ImageChangeParam)int.Parse(_))),
            NavigatePreviousMainImage = ReactiveCommand.Create<string>(_ => ChangeToPreviousImage((ImageChangeParam)int.Parse(_))),
            NavigateFirstImage = ReactiveCommand.Create<string>(_ => ChangeToPreviousImage((ImageChangeParam)int.Parse(_))),
            NavigateLastImage = ReactiveCommand.Create<string>(_ => ChangeToNextImage((ImageChangeParam)int.Parse(_))),
            ResetMainImagePosition = ReactiveCommand.Create(ResetImagePosition),
            SetImageFilteredValue = this.SetImageFilteredValue
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

        // ************************************************************************************
        // I also need to dispose of these old ref view models, they chill in memory too
        // Actually GC mostly handles this fine, it might balloon if tons of images were selected but I don't think 
        // that will happen + it would not really be large enough to care

        // Have to save the view models cause I need to notify the VM's that the collection changes
        // Other option is to just make a new VM every time..? Seems smelly to do that
        this.AlphaReferenceViewModel = new WorkspaceReferenceImageViewModel(this.CurrentAppState, this.ProjectConfig, AlphaReferenceImages, ReferenceViewIdentifier.Alpha);
        this.BetaReferenceViewModel = new WorkspaceReferenceImageViewModel(this.CurrentAppState, this.ProjectConfig, BetaReferenceImages, ReferenceViewIdentifier.Beta);

        WorkspaceAlphaReferenceRouter.Navigate.Execute(this.AlphaReferenceViewModel);
        WorkspaceBetaReferenceRouter.Navigate.Execute(this.BetaReferenceViewModel);

        // ************************************************************************************

        // Handle rebuilding the image split when reference image count changes
        this.ProjectConfig.ReferenceImages.CollectionChanged += ManageReferenceSplit;

        this.CloseOverlayView = ReactiveCommand.Create(_closeOverlayView);
        this.CloseProgressOverlayView = ReactiveCommand.Create(_closeProgressOverlayView);

        // ????? tuple shit ????? and it works ?????
        // This just creates the command, tuple cause funny
        SortImageDetailsAsyncCommand = ReactiveCommand.CreateFromObservable<(double Counter, double ImageCount), Unit>(tuple => SortImageDetails(tuple.Counter, tuple.ImageCount));

        this.WhenAnyValue(x => x.ProjectConfig.ReferenceImages).Subscribe(_ => BtnCommand());
    }
}
