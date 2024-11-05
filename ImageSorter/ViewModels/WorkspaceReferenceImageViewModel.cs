using Avalonia.Media.Imaging;
using ImageSorter.Models;
using ReactiveUI;
using System;

namespace ImageSorter.ViewModels;

public class WorkspaceReferenceImageViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; }
    public ImageDetails ImageDetails { get; }
    public string ImageName { get; }
    public string ImagePath { get; }
    public string FilterValue { get; set; }

    private Bitmap _imageBitmap;
    public Bitmap ImageBitmap
    {
        get { return _imageBitmap; }
        protected set { this.RaiseAndSetIfChanged(ref _imageBitmap, value); }
    }

    private bool _imageExists = false;
    public bool ImageExists
    {
        get { return _imageExists; }
        protected set { this.RaiseAndSetIfChanged(ref _imageExists, value); }
    }

    public void SetReferenceImage()
    {

    }


    public WorkspaceReferenceImageViewModel(ImageDetails imageDetails, string FilterValue)
    {
        //this.ImageDetails = imageDetails;
        //this.ImagePath = imageDetails.FilePath;
        //this.ImageName = imageDetails.FileName;
        //this.FilterValue = FilterValue;
        UrlPathSegment = $"WorkspaceReferenceImage[{FilterValue}]";

        if (!string.IsNullOrEmpty(this.ImagePath))
        {
            ImageBitmap = new Bitmap(this.ImagePath);
            ImageExists = true;
        }
    }

}