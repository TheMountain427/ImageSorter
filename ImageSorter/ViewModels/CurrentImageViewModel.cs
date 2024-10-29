

using Avalonia.Media.Imaging;
using ImageSorter.Models;

namespace ImageSorter.ViewModels;

public class CurrentImageViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } 

    public string ImageName { get; }

    public string ImagePath { get; }

    public Bitmap ImageBitmap { get; }

    public int CurrentIndex { get; }

    public CurrentImageViewModel(ImageDetails imageDetails, int CurrentIndex)
    {
        this.ImagePath = imageDetails.FilePath;
        this.CurrentIndex = CurrentIndex;
        this.ImageName = imageDetails.FileName;
        UrlPathSegment = $"CurrentImage[{CurrentIndex}]";
        ImageBitmap = new Bitmap(this.ImagePath);
    }
}