using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using static ImageSorter.Models.Enums;

namespace ImageSorter.ViewModels;

public class WorkspaceReferenceImageViewModel : ViewModelBase
{
    public override string? UrlPathSegment { get; }

    public ProjectConfig ProjectConfig { get; }

    public string DefaultImgPath { get; }

    public Bitmap DefaultBitmap { get; }

    public ReferenceViewIdentifier ReferenceViewID { get; }

    private ObservableCollection<ImageDetails> _referenceImages;
    public ObservableCollection<ImageDetails> ReferenceImages
    {
        get { return _referenceImages; }
        protected set { this.RaiseAndSetIfChanged(ref _referenceImages, value); }
    }

    // I believe all this image loading and reference stuff can be improved and simplified
    // with proper obeservable collection setup and event handling.
    // ImageDetails propbably needs more RaiseAndSetIfChanged on filepath and iamge properties.
    // It works as the moment so I do not feel like messing with it.

    // ????
    public ReactiveCommand<int, Unit> SetReferenceImage { get; }

    private async void _setReferenceImage(int referenceIndex)
    {
        var selectedFile = await BrowseFilesReference();
        UpdateReferenceImage(referenceIndex, selectedFile);


        //There is def a better way than rebuilding the whole collection when loading a new image
        var tempImgRef = new ImageDetails[this.ReferenceImages.Count];
        ReferenceImages.CopyTo(tempImgRef, 0);
        UpdateReferenceCollection(tempImgRef);
    }


    private async Task<IReadOnlyList<IStorageFile>?> BrowseFilesReference()
    {
        // Set settings for folder browser, default location is Desktop
        var options = new FilePickerOpenOptions
        {
            AllowMultiple = false,
            SuggestedStartLocation = await App.TopLevel.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Desktop),
            FileTypeFilter = [FilePickerFileTypes.ImageAll],
            Title = "Select Reference Image"
        };

        // Try to use the last location used to select reference images
        if (!string.IsNullOrEmpty(this.CurrentAppState.LastReferenceImagePath))
        {
            var tryGetFolder = await App.TopLevel.StorageProvider.TryGetFolderFromPathAsync(this.CurrentAppState.LastReferenceImagePath);
            options.SuggestedStartLocation = tryGetFolder is not null ? tryGetFolder : options.SuggestedStartLocation;
        }

        var selectedFile = await App.TopLevel.StorageProvider.OpenFilePickerAsync(options);

        return selectedFile;

    }

    private void UpdateReferenceImage(int referenceIndex, IReadOnlyList<IStorageFile>? selectedFile)
    {
        if (selectedFile is not null && selectedFile.Count != 0)
        {
            var selectedFilePath = selectedFile[0].Path.LocalPath;

            var referenceImageDetail = this.ReferenceImages.FirstOrDefault(x => x.ImageIndex == referenceIndex
                                                                        && x.ReferenceViewID == this.ReferenceViewID);

            if (referenceImageDetail is not null)
            {
                referenceImageDetail.FilePath = selectedFilePath;

                var selectedFileParentDir = Directory.GetParent(selectedFilePath);

                // Save for convience when browsing for the next reference image
                if (selectedFileParentDir is not null && !string.IsNullOrEmpty(selectedFileParentDir.FullName))
                {
                    this.CurrentAppState.LastReferenceImagePath = selectedFileParentDir.FullName;
                }
            }
            else
            {
                throw new ArgumentNullException($"No reference image found with referenceIndex, {referenceIndex}");
            }
        }
    }

    // Handle when number of reference image changes by calling image loader
    // This is triggering 4 times on a collection change.
    // Actually I'm a partial dingus, have two views using this so it needs to run twice.
    // Other 2 are reset events due to my smelly code, will filter those.
    // Actually actually, runs on each foreach in UpdateReferenceCollection. Only need it run at the end. <- Fixed
    private void LoadReferenceBitmapOnChange(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Filter out Resets, since we are doing a Clear() then loading all images
        if (e.Action != NotifyCollectionChangedAction.Reset)
        {
            LoadReferenceBitmap();
        }
    }


    // Load bitmap for references, default image for null paths
    private void LoadReferenceBitmap()
    {
        if (this.ReferenceImages.Count > 0)
        {
            foreach (var refImg in this.ReferenceImages)
            {
                if (!string.IsNullOrEmpty(refImg.FilePath)
                    && (refImg.FilePath.EndsWith(".jpeg") || refImg.FilePath.EndsWith(".png") || refImg.FilePath.EndsWith(".jpg"))
                    && File.Exists(refImg.FilePath))
                {
                    refImg.ImageNotLoaded = false;
                    refImg.ImageBitmap = new Bitmap(refImg.FilePath);

                    // Fill out image properties
                    refImg.ValidateImageProperties();
                }
                else
                {
                    refImg.ImageNotLoaded = true;
                    refImg.ImageBitmap = DefaultBitmap;
                }
            }
        }
    }

    // Should probably change this to reactive once ProjectConfig.ReferenceImages is changed to reactive
    public void UpdateReferenceCollection(IEnumerable<ImageDetails> referenceImages)
    {
        // ObservableCollections suck and I am lazy
        var tmpReferenceImages = new ObservableCollection<ImageDetails>(this.ReferenceImages);
        this.ReferenceImages.Clear();

        // Going to do a little hacky thing to prevent this from triggering LoadReferenceBitmapOnChange
        // multiple times when WorkspaceViewModel provides images with count > 1
        if (referenceImages.Count() > 0)
        {
            int i = 0;
            // Ubsubscribe from collection changed for now
            this.ReferenceImages.CollectionChanged -= LoadReferenceBitmapOnChange;

            foreach (var refImg in referenceImages)
            {
                // If we are at the last refImg, subsribe again to trigger an image load on all images
                if (i + 1 == referenceImages.Count())
                {
                    this.ReferenceImages.CollectionChanged += LoadReferenceBitmapOnChange;
                }

                // Triggers the LoadReferenceBitmapOnChange on last refImg to update everything 
                this.ReferenceImages.Add(refImg);

                refImg.ReferenceViewID = this.ReferenceViewID;

                // Set ImageIndexes as index + 1 of the original reference images collection.
                // This is used for keybinding tooltips
                refImg.ImageIndex = this.ProjectConfig.ReferenceImages.IndexOf(refImg) + 1;
                i++;
            }
        }
    }

    public WorkspaceReferenceImageViewModel(AppState CurrentAppState, ProjectConfig projectConfig, IEnumerable<ImageDetails> referenceImages,
                                            ReferenceViewIdentifier referenceViewID) : base(CurrentAppState)
    {
        this.CurrentAppState = CurrentAppState;
        this.ProjectConfig = projectConfig;
        this.ReferenceViewID = referenceViewID;

        this.SetReferenceImage = ReactiveCommand.Create<int>(_setReferenceImage);

        // Use provided referenceImage as it may be modified version of ProjectConfig
        this._referenceImages = new ObservableCollection<ImageDetails>(referenceImages);

        this.DefaultImgPath = Path.Join(Environment.CurrentDirectory, @"\Assets\512x512-Transparent.png");
        this.DefaultBitmap = new Bitmap(DefaultImgPath);

        // Load ref images, reload preexisting images if they exist, default image if not
        LoadReferenceBitmap();

        this.ReferenceImages.CollectionChanged += LoadReferenceBitmapOnChange;
    }

}