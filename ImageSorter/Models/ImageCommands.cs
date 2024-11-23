using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageSorter.Models;

public class ImageCommands
{
    public ICommand NavigateNextMainImage { get; set; }
    public ICommand NavigatePreviousMainImage { get; set; }
    public ICommand NavigateLastImage { get; set; }
    public ICommand NavigateFirstImage { get; set; }
    public ICommand ResetMainImagePosition { get; set; }
    public ICommand SetImageFilteredValue { get; set; }
    public ICommand BeginImageSorting { get; set; }
    public ICommand BrowseForNewOutput { get; set; }

    // To change the sort order of the images
    // Yes, it seems like too much to send but every other way blows up or fails
    public ICommand ChangeImageSortOrder { get; set; }
    public ImgOrderOptions ImageOrderOptions { get; set; }
    public ImgOrderOption ImageSortOrder { get; set; }

    public ImageCommands()
    {
    }
}

