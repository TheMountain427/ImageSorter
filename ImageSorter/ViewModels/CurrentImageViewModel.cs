

using Avalonia.Media.Imaging;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace ImageSorter.ViewModels;

public class CurrentImageViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public override string UrlPathSegment { get; }
    public ImageDetails ImageDetails { get; }
    public string ImageName { get; }
    public string ImagePath { get; }

    // This needs to be disposed when finished with. Call Dispose()
    public Bitmap ImageBitmap { get; private set; }

    public int CurrentIndex { get; }

    public CurrentImageViewModel(ImageDetails imageDetails, int CurrentIndex)
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


        this.ImageDetails = imageDetails;
        this.ImagePath = imageDetails.FilePath;
        this.CurrentIndex = CurrentIndex;
        this.ImageName = imageDetails.FileName;
        UrlPathSegment = $"CurrentImage[{CurrentIndex}]";
        ImageBitmap = new Bitmap(imageDetails.LoadImageBitmap());

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