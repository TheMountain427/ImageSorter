

using Avalonia.Media.Imaging;

namespace ImageSorter.ViewModels;

public class CurrentImageViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } 

    public string ImagePath { get; }

    public Bitmap ImageBitmap { get; }

    public int CurrentIndex { get; }

    public CurrentImageViewModel(string ImagePath, int CurrentIndex)
    {
        this.ImagePath = ImagePath;
        this.CurrentIndex = CurrentIndex;
        UrlPathSegment = $"CurrentImage[{CurrentIndex}]";
        ImageBitmap = new Bitmap(this.ImagePath);
    }
}