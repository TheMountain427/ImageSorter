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
    public ICommand ResetMainImagePosition { get; set; }


    public ImageCommands()
    {
    }
}

