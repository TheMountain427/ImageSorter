using ImageSorter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter.ViewModels;

public class WorkspaceThumbnailViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "WorkspaceThumbnailView";

    public List<ImageDetails> ImageDetails { get; set; }


    public void SetReferenceImage(object args)
    {

    }

    public void ChangeFilterName(object args)
    {

    }
}

