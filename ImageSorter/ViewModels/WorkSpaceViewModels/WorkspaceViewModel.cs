using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ImageSorter.Models;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using static ImageSorter.Models.Enums;
using static ImageSorter.Models.Helpers;
using static ImageSorter.Models.SortOptionKey;

namespace ImageSorter.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{

    public override string UrlPathSegment { get; } = "WorkspaceView";

    public ProjectConfig ProjectConfig { get; protected set; }

    private AppState _aps { get; }

    public RoutingState CurrentImageRouter { get; } = new RoutingState();

    public RoutingState WorkspaceControlsRouter { get; } = new RoutingState();

    private List<ImageDetails> _sortedImageDetails;
    public List<ImageDetails> SortedImageDetails
    {
        get { return _sortedImageDetails; }
        set { this.RaiseAndSetIfChanged(ref _sortedImageDetails, value); }
    }

    private IQueryable<ImageDetails> _baseImageDetails { get; set; }

    public RoutingState WorkspaceFilterRouter { get; } = new RoutingState();

    public RoutingState WorkspaceAlphaReferenceRouter { get; } = new RoutingState();

    public RoutingState WorkspaceBetaReferenceRouter { get; } = new RoutingState();

    public RoutingState OverlayRouter { get; } = new RoutingState();

    public RoutingState ThumbnailRouter { get; } = new RoutingState();

    public List<ImageDetails> AlphaReferenceImages { get; set; } = new List<ImageDetails>();

    public List<ImageDetails> BetaReferenceImages { get; set; } = new List<ImageDetails>();

    public WorkspaceReferenceImageViewModel AlphaReferenceViewModel { get; }

    public WorkspaceReferenceImageViewModel BetaReferenceViewModel { get; }

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

    public ZoomBorder? ZoomBorder { get; protected set; }

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

    public CurrentImageViewModel NextImageVM { get; protected set; }
    public CurrentImageViewModel CurrentImageVM { get; protected set; }
    public CurrentImageViewModel PreviousImageVM { get; protected set; }

    private void UpdateImageSortOrder(ImgOrderOption? ImgOrderOption)
    {
        // Prevent this from running on workspace load
        if (ImgOrderOption is not null && this.CurrentImageVM is not null)
        {
            // Sort order has changed
            this.SortedImageDetails = SortImageDetailsBy(this._baseImageDetails, ImgOrderOption.OptionEnum).ToList();

            // Find our current main image in the sort index so we can return to it
            var newCurrentImageDetailsIndex = SortedImageDetails.IndexOf(this.CurrentImageVM.ImageDetails);

            // We found it
            if (SortedImageDetails[newCurrentImageDetailsIndex] is not null)
            {
                this.CurrentImageIndex = newCurrentImageDetailsIndex;
            }
            // Aw fuck, it's gone. Back to 0.
            else
            {
                this.CurrentImageIndex = 0;
            }

        }
    }

    // Now you don't have to think about it
    public void ChangeToNextImage()
    {
        if (this.CurrentImageIndex < this.SortedImageDetails.Count - 1)
        {
            this.CurrentImageIndex++;
        }
    }

    public void ChangeToPreviousImage()
    {
        if (this.CurrentImageIndex > 0 && this.SortedImageDetails.Count - 1 > 0)
        {
            this.CurrentImageIndex--;
        }

    }

    public void GoToLastImage()
    {
        if (this.CurrentImageIndex != this.SortedImageDetails.Count - 1)
        {
            this.CurrentImageIndex = this.SortedImageDetails.Count - 1;
        }
    }

    public void GoToFirstImage()
    {
        if (this.CurrentImageIndex != 0)
        {
            this.CurrentImageIndex = 0;
        }
    }

    public void ResetImagePosition()
    {
        this.ZoomBorder?.ResetMatrix();
    }

    public void SetImageFilterValue(string FilterValue)
    {
        // Not sure why I did it like this
        this.ProjectConfig.SetImageFilterValue(CurrentImageVM.ImageDetails, FilterValue);
    }

    private void ManageReferenceSplit(object? sender, EventArgs e)
    {
        switch (this.CurrentAppState.ReferenceSplitSetting)
        {
            case ReferenceSplit.Split:
                {
                    // Make alpha have the greater number of refs if the total reference count is odd
                    int alphaCount = this.ProjectConfig.ReferenceImages.Count / 2 + (this.ProjectConfig.ReferenceImages.Count % 2);

                    this.AlphaReferenceImages = this.ProjectConfig.ReferenceImages.Take(alphaCount).ToList();
                    this.BetaReferenceImages = this.ProjectConfig.ReferenceImages.Skip(alphaCount).ToList();
                }
                break;
            case ReferenceSplit.Alpha:
                {
                    // All on Alpha reference view
                    this.AlphaReferenceImages = this.ProjectConfig.ReferenceImages.ToList();
                    this.BetaReferenceImages = new List<ImageDetails>();
                }
                break;
            case ReferenceSplit.Beta:
                {
                    // All on Beta reference view
                    this.AlphaReferenceImages = new List<ImageDetails>();
                    this.BetaReferenceImages = this.ProjectConfig.ReferenceImages.ToList();
                }
                break;
            default:
                break;
        }

        this.AlphaReferenceViewModel?.UpdateReferenceCollection(this.AlphaReferenceImages);
        this.BetaReferenceViewModel?.UpdateReferenceCollection(this.BetaReferenceImages);
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
            var unsortedCount = this.ProjectConfig.InputImages.Where(x => x.FilteredValue == "Unsorted").Count();

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
            var sortConfirmationVM = new SortConfirmationViewModel(_aps, SortConfirmations: sortConfirmations,
                                                                         CloseOverlay: this.CloseOverlayView,
                                                                         OnSuccessCommand: processWarningOptions,
                                                                         OnCancelCommand: cancelCommand);

            // Mark this as up, we will have a handler that will show preview if continue is selected
            this.IsSortWarningUp = true;

            // Create and navigate to the overlay view
            OverlayRouter.Navigate.Execute(new OverlayViewModel(_aps, ViewModelToDisplay: sortConfirmationVM,
                                                                      CloseOverlay: CloseOverlayView,
                                                                      AllowClickOff: false));
            // Subscribe to watch for OnSuccessCommand to run and mark IsSortWarningUp =
            this.OnIsSortWarningUpChange += ShowPreviewAfterWarning;
        }
        else
        {
            // No warnings, we can proceed to the sort preview
            ShowPreSortPreview();
        }
    }

    private void ShowPreviewAfterWarning(object? sender, EventArgs e)
    {
        // Show the sort preview
        ShowPreSortPreview();
        // Unsubscribe, we only want to be subscribed when the view is up
        this.OnIsSortWarningUpChange -= ShowPreviewAfterWarning;
    }

    private void ShowPreSortPreview()
    {
        var successCommand = ReactiveCommand.Create(() =>
        {
            BeginSortProcess();
        });

        var cancelCommand = ReactiveCommand.Create(() =>
        {
            this.CloseOverlayView.Execute(null);
        });

        // Do nothing on image click because we do not want to exit out of overview during sort confirmations
        ShowPreSortOverview(successCommand, cancelCommand, ReactiveCommand.Create<string>((_) => OverviewDoNothing(_)));

    }

    // For if showing overview but don't want image clicks to do anything
    private void OverviewDoNothing(string s)
    {

    }

    private bool TrySetMainIndexByFileName(string? FileName)
    {
        if (!string.IsNullOrEmpty(FileName))
        {
            var imageDetail = this.SortedImageDetails.FirstOrDefault(x => x.FileName == FileName);

            if (imageDetail is not null)
            {
                this.CurrentImageIndex = this.SortedImageDetails.IndexOf(imageDetail);
                return true;
            }
        }
        return false;
    }

    private void ShowOverview(ICommand successCommand, ICommand cancelCommand, ICommand? ImageClickCommand)
    {
        var imageClickCommand = ImageClickCommand is null ? null : ImageClickCommand;
        // Oh my god your constructor is huge
        var sortPreviewVM = new ImageOverviewViewModel(_aps, ProjectConfig: this.ProjectConfig,
                                                             SortedImageDetails: SortedImageDetails,
                                                             OnSuccessCommand: successCommand,
                                                             OnCancelCommand: cancelCommand,
                                                             ImageClickCommand: imageClickCommand,
                                                             ChangeSortOrder: ReactiveCommand.Create<ImgOrderOption>(_ => this.ImageSortOrder = _),
                                                             ImageOrderOptions: this.ImageOrderOptions,
                                                             ImageSortOrder: this.ImageSortOrder);

        var overlayVM = new OverlayViewModel(_aps, ViewModelToDisplay: sortPreviewVM,
                                                   CloseOverlay: CloseOverlayView,
                                                   AllowClickOff: true);

        // Pre-activating the VM prevents it from stuttering in
        sortPreviewVM.Activator.Activate();
        overlayVM.Activator.Activate();

        OverlayRouter.Navigate.Execute(overlayVM);
    }

    private void ShowPreSortOverview(ICommand successCommand, ICommand cancelCommand, ICommand? ImageClickCommand)
    {
        var imageClickCommand = ImageClickCommand is null ? null : ImageClickCommand;
        // Oh my god your constructor is huge
        var sortPreviewVM = new ImageOverviewViewModel(_aps, ProjectConfig: this.ProjectConfig,
                                                             SortedImageDetails: SortedImageDetails,
                                                             OnSuccessCommand: successCommand,
                                                             OnCancelCommand: cancelCommand,
                                                             ImageClickCommand: imageClickCommand,
                                                             ChangeSortOrder: ReactiveCommand.Create<ImgOrderOption>(_ => this.ImageSortOrder = _),
                                                             ImageOrderOptions: this.ImageOrderOptions,
                                                             ImageSortOrder: this.ImageSortOrder);

        var overlayVM = new OverlayViewModel(_aps, ViewModelToDisplay: sortPreviewVM,
                                                   CloseOverlay: CloseOverlayView,
                                                   AllowClickOff: false);

        // Pre-activating the VM prevents it from stuttering in
        sortPreviewVM.Activator.Activate();
        overlayVM.Activator.Activate();

        OverlayRouter.Navigate.Execute(overlayVM);
    }

    private void BeginSortProcess()
    {
        double imageCount = this.ProjectConfig.InputImages.Count;
        this.SortProgress = 0;
        var inProgessText = "Watch it! I'm trying to work here!";

        double maxProgress = this.SortEngine.CalculateSortProgressMaximum(this.ProjectConfig);

        var progressVM = new InProgressViewModel(_aps, MinimumValue: 0,
                                                       MaximumValue: maxProgress,
                                                       ValueToWatch: this.WhenAnyValue(x => x.SortProgress), // pass an IObservable<double>, so it can be monitored
                                                       InProgressText: inProgessText,
                                                       PauseOnCompletion: true,
                                                       OnSuccessCommand: this.CloseProgressOverlayView,
                                                       CompletionMessage: "I'm done!");

        OverlayRouter.Navigate.Execute(new OverlayViewModel(_aps, ViewModelToDisplay: progressVM,
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
    // Has no need to be a tuple, (in fact it is useless) but I am leaving it cause it's cool
    // The tuple allows more than one variable to be passed in
    public ReactiveCommand<(double Counter, double ImageCount), Unit> SortImageDetailsAsyncCommand { get; }
    // This is the method to be called by SortImageDetailsAsyncCommand
    // https://stackoverflow.com/questions/76400048/ui-freezing-while-executing-async-command-avaloniaui-reactiveui
    private IObservable<Unit> SortImageDetails(double Counter, double ImageCount)
    {
        return Observable.Start(() =>
        {
            // Seperate the images into groups based on filters
            var sortedImageGroups = this.SortEngine.SortImageDetailsForOutput(this.ProjectConfig.InputImages, this.ProjectConfig.ReferenceImages, this.SortConfigs);

            // Get the filters that are actually being used since I didn't think far enough ahead
            var outputNames = this.SortEngine.ConvertSortedGroupsToFilterOutputDirectories(sortedImageGroups);

            // Create the output directories the images will go into
            var outputDirectories = this.SortEngine.CreateOutputDirectories(outputNames, this.ProjectConfig.OutputDirectoryPaths.First());

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

    public void LoadThumbnails(IEnumerable<ImageDetails> ImageDetails)
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

    public void Dbg_GoToProjectSelection()
    {
        if (_aps.DebugMode)
        {
            _goToProjectSelectionByName();
        }
    }

    private void _goToProjectSelection()
    {
        MainRouter.NavigateAndReset.Execute(new ProjectSelectionViewModel(_aps, this.HostScreen, this.MainRouter));
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

    private void ShiftCurrentIndexByValue(int shift)
    {
        var potentialIndex = this.CurrentImageIndex + shift;

        // Stay within bounds
        if (potentialIndex >= 0 && potentialIndex < this.SortedImageDetails.Count)
        {
            this.CurrentImageIndex = potentialIndex;
        }
    }

    private void GoToImageFromOverview(string FileName)
    {
        if (this.TrySetMainIndexByFileName(FileName))
        {
            //OverlayRouter.NavigationStack.Clear();
            this.CloseOverlayView.Execute(null);
        }
    }

    public void OpenOverview()
    {
        var successCommand = ReactiveCommand.Create(() =>
        {
            this.CloseOverlayView.Execute(null);
        });

        var cancelCommand = ReactiveCommand.Create(() =>
        {
            this.CloseOverlayView.Execute(null);
        });

        ShowOverview(successCommand, cancelCommand, ReactiveCommand.Create<string>(_ => GoToImageFromOverview(_)));
    }

    public void Dbg_Overview()
    {
        var successCommand = ReactiveCommand.Create(() =>
        {
            this.CloseOverlayView.Execute(null);
        });

        var cancelCommand = ReactiveCommand.Create(() =>
        {
            this.CloseOverlayView.Execute(null);
        });


        ShowOverview(successCommand, cancelCommand, ReactiveCommand.Create<string>(_ => GoToImageFromOverview(_)));
    }

    public void OpenDefaultOverview()
    {
        var successCommand = ReactiveCommand.Create(() =>
        {
            this.CloseOverlayView.Execute(null);
        });

        var cancelCommand = ReactiveCommand.Create(() =>
        {
            this.CloseOverlayView.Execute(null);
        });


        ShowOverview(successCommand, cancelCommand, ReactiveCommand.Create<string>(_ => GoToImageFromOverview(_)));
    }

    private void ChangeMainImage(int NewCurrentImageIndex)
    {
        // CurrentIndex changed, now find lets find the ViewModel that has the index

        // Check if the NextImageVM has the index, means next image was requested
        if (NextImageVM is not null && NewCurrentImageIndex == this.NextImageVM.CurrentIndex && this.NextImageVM?.CanNavigateTo == true)
        {
            IncrementMainImageView();
        }

        // NextImageVM does not have it, maybe previous image was requested. Check PreviousVM for the index.
        else if (PreviousImageVM is not null && NewCurrentImageIndex == this.PreviousImageVM.CurrentIndex && this.PreviousImageVM.CanNavigateTo == true)
        {
            DecrementMainImageView();
        }

        // Oh no, neither of our cache vm's have the index. We have to load new ones for all three.
        // Either SortedImageDetails order changed, SortedImageDetails collection was changed, or we are skipping images (like go to selected image, or last image)
        // Check that the index is valid first.
        else if (NewCurrentImageIndex >= 0 && NewCurrentImageIndex <= SortedImageDetails.Count - 1)
        {
            ReinitializeMainImageVM();
        }
        else
        {
            // Well are index either didn't actually change or the index was invalid
            // throw?
            // Just reset currentindex to 0, this should handle if the Project Config has an out of bounds index as well
            Debug.WriteLine($"Supplied CurrentImageIndex{NewCurrentImageIndex} is out of bounds of 0-{SortedImageDetails.Count}. Resetting index to 0.");
            this.CurrentImageIndex = 0;
            ReinitializeMainImageVM();
        }

        void IncrementMainImageView()
        {
            // Slide to the right...
            this.CurrentImageRouter.Navigate.Execute(this.NextImageVM);
            this.PreviousImageVM?.Dispose();
            this.PreviousImageVM = CurrentImageVM;
            this.CurrentImageVM = NextImageVM;
            this.NextImageVM = new CurrentImageViewModel(_aps, this.SortedImageDetails, this.CurrentImageIndex + 1);
        }

        void DecrementMainImageView()
        {
            // Slide to the left...
            this.CurrentImageRouter.Navigate.Execute(this.PreviousImageVM);
            this.NextImageVM?.Dispose();
            this.NextImageVM = this.CurrentImageVM;
            this.CurrentImageVM = this.PreviousImageVM;
            this.PreviousImageVM = new CurrentImageViewModel(_aps, this.SortedImageDetails, this.CurrentImageIndex - 1);
        }

        void ReinitializeMainImageVM()
        {
            // Criss-Cross! Now everybody clap your hands!

            // Use the NextImageVM to load our new VM
            this.NextImageVM?.Dispose();
            this.NextImageVM = new CurrentImageViewModel(_aps, this.SortedImageDetails, this.CurrentImageIndex);

            // Just in case. Should never happen though (I think)
            if (this.NextImageVM.CanNavigateTo == false)
                throw new ArgumentException("Blew the fuck up, main image navigator broke bad. Probably an index issue");

            // Yay! We found our new spot!
            this.CurrentImageRouter.Navigate.Execute(this.NextImageVM);

            // Clean out the old vm's
            this.CurrentImageVM?.Dispose();
            this.PreviousImageVM?.Dispose();

            // Swap over Next to Current
            this.CurrentImageVM = this.NextImageVM;

            // Set up new next and previous (if able)
            this.NextImageVM = new CurrentImageViewModel(_aps, this.SortedImageDetails, this.CurrentImageIndex + 1);
            this.PreviousImageVM = new CurrentImageViewModel(_aps, this.SortedImageDetails, this.CurrentImageIndex - 1);
        }
    }



    private async void BrowseAndSelectNewOutput()
    {
        // Set settings for folder browser, default location is Desktop
        var options = new FolderPickerOpenOptions
        {
            AllowMultiple = true,
            SuggestedStartLocation = await App.TopLevel.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Desktop),
            Title = "Select Output Directory"
        };

        // Try to use the current output directory as the starting location
        if (!string.IsNullOrEmpty(this.CurrentAppState.LastReferenceImagePath))
        {
            var tryGetFolder = await App.TopLevel.StorageProvider.TryGetFolderFromPathAsync(this.ProjectConfig.OutputDirectoryPaths[0]);
            options.SuggestedStartLocation = tryGetFolder is not null ? tryGetFolder : options.SuggestedStartLocation;
        }

        // Haha, we let them pick more than one output directory but we actually only use the first to sort lmaooooooooooo
        var SelectedDirectories = await App.TopLevel.StorageProvider.OpenFolderPickerAsync(options);

        var SelectedDirectoriesPaths = new List<string>();

        if (SelectedDirectories is not null)
        {
            SelectedDirectoriesPaths.AddRange(SelectedDirectories.Select(x => x.Path.LocalPath));
        }

        this.ProjectConfig.OutputDirectoryPaths = SelectedDirectoriesPaths;
    }

    private void CurrentImageZoomBorder_KeyDown(object? sender, KeyEventArgs e)
    {
        HandleZoomBorderHotKey(sender, e, this.ZoomBorder);
    }

    public void SetZoomBorder(ZoomBorder? zm)
    {
        if (zm is not null)
        {
            this.ZoomBorder = zm;
            ZoomBorder.KeyDown += CurrentImageZoomBorder_KeyDown;
        }
    }

    private void HandleZoomBorderHotKey(object? sender, RoutedEventArgs e, object? item)
    {
        if (sender is not null && item is ZoomBorder zm && e is KeyEventArgs ke)
        {
            switch (ke.Key)
            {
                case Key.F:
                    zm?.Fill();
                    break;
                case Key.U:
                    zm?.Uniform();
                    break;
                case Key.R:
                    zm?.ResetMatrix();
                    break;
                case Key.T:
                    zm?.ToggleStretchMode();
                    zm?.AutoFit();
                    break;
                case Key.Right:
                    if (ke.KeyModifiers == KeyModifiers.Control)
                        GoToLastImage();
                    else
                        ChangeToNextImage();
                    break;
                case Key.Left:
                    if (ke.KeyModifiers == KeyModifiers.Control)
                        GoToFirstImage();
                    else
                        ChangeToPreviousImage();
                    break;
                case Key.OemPeriod or Key.Decimal:
                    SetImageFilterValue("Unsorted");
                    break;
                case Key.D1 or Key.NumPad1:
                    SetFilterFromHotkey(1);
                    break;
                case Key.D2 or Key.NumPad2:
                    SetFilterFromHotkey(2);
                    break;
                case Key.D3 or Key.NumPad3:
                    SetFilterFromHotkey(3);
                    break;
                case Key.D4 or Key.NumPad4:
                    SetFilterFromHotkey(4);
                    break;
                case Key.D5 or Key.NumPad5:
                    SetFilterFromHotkey(5);
                    break;
                case Key.D6 or Key.NumPad6:
                    SetFilterFromHotkey(6);
                    break;
                case Key.D7 or Key.NumPad7:
                    SetFilterFromHotkey(7);
                    break;
                case Key.D8 or Key.NumPad8:
                    SetFilterFromHotkey(8);
                    break;
                case Key.D9 or Key.NumPad9:
                    SetFilterFromHotkey(9);
                    break;
            }
        }

        void SetFilterFromHotkey(int index)
        {
            if (index - 1 < this.ProjectConfig.ReferenceImages.Count)
            {
                SetImageFilterValue(this.ProjectConfig.ReferenceImages[index - 1].FilteredValue);
            }
        }
    }

    public WorkspaceViewModel(IScreen screen, RoutingState router, AppState CurrentAppState, ProjectConfig projectConfig) : base(CurrentAppState)
    {
        _aps = this.CurrentAppState;
        this.MainRouter = router;
        this.HostScreen = screen;
        this.ProjectConfig = projectConfig;
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

        this._baseImageDetails = this.ProjectConfig.InputImages.AsQueryable();

        this.SortedImageDetails = SortImageDetailsBy(this._baseImageDetails, this.ImageSortOrder.OptionEnum).ToList();
        this.WhenAnyValue(x => x.ImageSortOrder).Subscribe(_ => UpdateImageSortOrder(_));

        // Setup the MainImage. ChangeMainImage() and CurrentImageVM handle it all based on index and SortedImageDetails
        // Isn't that nice?
        if (SortedImageDetails is not null)
        {
            this.CurrentImageIndex = this.ProjectConfig.CurrentImageIndex;
            this.WhenAnyValue(x => x.CurrentImageIndex).Subscribe(_ => ChangeMainImage(_));
        }
        else
        {
            throw new ArgumentException("No images were supplied ya dingus.");
        }

        // This should change at some point. Really shouldn't load all of them into memory tbh.
        if (_baseImageDetails is not null)
        {
            LoadThumbnails(this._baseImageDetails);
        }

        SetImageFilteredValue = ReactiveCommand.Create<string>(_ => SetImageFilterValue(_));

        // Pass through commands to control vm that navigate the main image
        var imageCommands = new ImageCommands()
        {
            NavigateNextMainImage = ReactiveCommand.Create<string>(_ => ChangeToNextImage()),
            NavigatePreviousMainImage = ReactiveCommand.Create<string>(_ => ChangeToPreviousImage()),
            NavigateLastImage = ReactiveCommand.Create<string>(_ => GoToLastImage()),
            NavigateFirstImage = ReactiveCommand.Create<string>(_ => GoToFirstImage()),
            ResetMainImagePosition = ReactiveCommand.Create(() => ResetImagePosition()),
            SetImageFilteredValue = this.SetImageFilteredValue,
            BeginImageSorting = ReactiveCommand.Create(() => CheckForImageSortIssues()),
            BrowseForNewOutput = ReactiveCommand.Create(() => BrowseAndSelectNewOutput()),
            OpenOverview = ReactiveCommand.Create(() => OpenDefaultOverview()),
            ChangeImageSortOrder = ReactiveCommand.Create<ImgOrderOption>(_ => this.ImageSortOrder = _),
            ImageOrderOptions = this.ImageOrderOptions,
            ImageSortOrder = this.ImageSortOrder
        };

        WorkspaceControlsRouter.Navigate.Execute(new WorkspaceControlsViewModel(_aps, this.ProjectConfig, imageCommands));

        // Side pane that allows modification of filter amounts
        WorkspaceFilterRouter.Navigate.Execute(new WorkspaceFilterViewModel(_aps, this.ProjectConfig));

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

        // ************************************************************************************
        // I also need to dispose of these old ref view models, they chill in memory too
        // Actually GC mostly handles this fine, it might balloon if tons of images were selected but I don't think
        // that will happen + it would not really be large enough to care

        // Have to save the view models cause I need to notify the VM's that the collection changes
        // Other option is to just make a new VM every time..? Seems smelly to do that
        this.WhenAnyValue(x => x.CurrentAppState.ReferenceSplitSetting).Subscribe(_ => ManageReferenceSplit(this, EventArgs.Empty));

        this.AlphaReferenceViewModel = new WorkspaceReferenceImageViewModel(_aps, this.ProjectConfig, AlphaReferenceImages, ReferenceViewIdentifier.Alpha);
        this.BetaReferenceViewModel = new WorkspaceReferenceImageViewModel(_aps, this.ProjectConfig, BetaReferenceImages, ReferenceViewIdentifier.Beta);

        WorkspaceAlphaReferenceRouter.Navigate.Execute(this.AlphaReferenceViewModel);
        WorkspaceBetaReferenceRouter.Navigate.Execute(this.BetaReferenceViewModel);
        this.ManageReferenceSplit(this, EventArgs.Empty);

        // ************************************************************************************

        // Handle rebuilding the image split when reference image count changes
        this.ProjectConfig.ReferenceImages.CollectionChanged += ManageReferenceSplit;

        this.CloseOverlayView = ReactiveCommand.Create(_closeOverlayView);
        this.CloseProgressOverlayView = ReactiveCommand.Create(_closeProgressOverlayView);

        // ????? tuple shit ????? and it works ?????
        // This just creates the command, tuple cause funny
        SortImageDetailsAsyncCommand = ReactiveCommand.CreateFromObservable<(double Counter, double ImageCount), Unit>(tuple => SortImageDetails(tuple.Counter, tuple.ImageCount));

        this.WhenAnyValue(x => x.ProjectConfig.ReferenceImages).Subscribe(_ => BtnCommand());

        var thumbnailVM = new WorkspaceThumbnailViewModel(_aps, CurrentImageIndex: this.CurrentImageIndex,
                                                                SortedImageDetails: this.SortedImageDetails,
                                                                CurrentImageIndexObservable: this.WhenAnyValue(x => x.CurrentImageIndex),
                                                                SortedImageDetailsObservable: this.WhenAnyValue(x => x.SortedImageDetails),
                                                                ImageShiftCommand: ReactiveCommand.Create<int>(_ => ShiftCurrentIndexByValue(_)));

        thumbnailVM.Activator.Activate();
        ThumbnailRouter.Navigate.Execute(thumbnailVM);
    }
}