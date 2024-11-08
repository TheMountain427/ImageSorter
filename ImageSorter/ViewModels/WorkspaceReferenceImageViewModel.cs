using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ImageSorter.ViewModels;

public class WorkspaceReferenceImageViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; }

    public ProjectConfig ProjectConfig { get; }

    public string DefaultImgPath { get; }

    public Bitmap DefaultBitmap { get; }

    private ObservableCollection<ImageDetails> _referenceImages;
    public ObservableCollection<ImageDetails> ReferenceImages
    {
        get { return _referenceImages; }
        protected set { this.RaiseAndSetIfChanged(ref _referenceImages, value); }
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

    public void SetFilterValue()
    {

    }

    // Handle when number of reference image changes by calling image loader
    private void LoadReferenceBitmapOnChange(object sender, EventArgs e)
    {
        LoadReferenceBitmap();
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
                    refImg.ImageBitmap = new Bitmap(refImg.FilePath);
                }
                else
                {
                    refImg.ImageBitmap = DefaultBitmap;
                }
            }
        }
    }

    public void UpdateReferenceCollection(IEnumerable<ImageDetails> referenceImages)
    {
        // ObservableCollections suck and I am lazy
        //var tmpReferenceImages = new ObservableCollection<ImageDetails>(this.ReferenceImages);
        this.ReferenceImages.Clear();
        if (referenceImages.Count() > 0)
        {
            foreach (var smellyImages in referenceImages)
            {
                this.ReferenceImages.Add(smellyImages);
            }
        }
    }

    public WorkspaceReferenceImageViewModel(ProjectConfig projectConfig, IEnumerable<ImageDetails> referenceImages)
    {
        this.ProjectConfig = projectConfig;

        // Use provided referenceImage as it may be modified version of ProjectConfig
        this.ReferenceImages = new ObservableCollection<ImageDetails>(referenceImages);

        this.DefaultImgPath = Path.Join(Environment.CurrentDirectory, @"\Assets\512x512-Transparent.png");
        this.DefaultBitmap = new Bitmap(DefaultImgPath);

        // Load ref images, reload preexisting images if they exist, default image if not
        LoadReferenceBitmap();

        this.ReferenceImages.CollectionChanged += LoadReferenceBitmapOnChange;
    }

}