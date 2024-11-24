using Avalonia.Media.Imaging;
using ImageSorter.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace ImageSorter.ViewModels;

public class CurrentImageViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public override string UrlPathSegment { get; }

    public ImageDetails? ImageDetails { get; }

    public string? ImageName { get; }

    // This needs to be disposed when finished with. Call Dispose()
    public Bitmap? ImageBitmap { get; private set; }

    public int CurrentIndex { get; }

    public bool CanNavigateTo { get; private set; }

    public CurrentImageViewModel(AppState CurrentAppState, List<ImageDetails> SortedImageDetails, int CurrentIndex) : base (CurrentAppState)
    {
        // Need to dispose of the bitmap since it is an IDisposble
        // Will balloon memory if not disposed
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable
                .Create(() => this.HandleDeactivation())
                .DisposeWith(disposables);
        });

        // Checking if their is a next image to load in the list is handled here now
        if (CurrentIndex < SortedImageDetails.Count && CurrentIndex >= 0)
        {
            this.ImageDetails = SortedImageDetails[CurrentIndex];
            this.CurrentIndex = CurrentIndex;
            this.ImageName = this.ImageDetails.FileName;
            ImageBitmap = new Bitmap(this.ImageDetails.LoadImageBitmap());

            // This boolean should be checked before attempting to navigate to this VM
            this.CanNavigateTo = true;
        }

        UrlPathSegment = $"CurrentImage[{CurrentIndex}]";

    }

    private void HandleDeactivation()
    {
        // Not disposing here because it will dispose the image too early
        // I am keeping the previous/next vm in variables as a sudo "cache" to prevent image flashing on load
    }

    // Have to manually call this from another view
    public void Dispose()
    {
        ImageBitmap?.Dispose();
        ImageBitmap = null;
    }
}