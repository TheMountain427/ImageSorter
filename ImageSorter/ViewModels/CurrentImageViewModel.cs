

using Avalonia.Media.Imaging;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace ImageSorter.ViewModels;

public class CurrentImageViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public override string UrlPathSegment { get; }
    public ImageDetails? ImageDetails { get; }
    public string? ImageName { get; }
    public string? ImagePath { get; }

    // This needs to be disposed when finished with. Call Dispose()
    public Bitmap? ImageBitmap { get; private set; }

    public int CurrentIndex { get; }

    public bool CanNavigateTo { get; private set; }

    public CurrentImageViewModel(List<ImageDetails> SortedImageDetails, int CurrentIndex)
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

        if (CurrentIndex < SortedImageDetails.Count && CurrentIndex >= 0)
        {
            this.ImageDetails = SortedImageDetails[CurrentIndex];
            this.CurrentIndex = CurrentIndex;
            this.ImagePath = this.ImageDetails.FilePath;
            this.ImageName = this.ImageDetails.FileName;
            ImageBitmap = new Bitmap(this.ImageDetails.LoadImageBitmap());
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