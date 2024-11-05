using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using static ImageSorter.Models.Helpers;

namespace ImageSorter.ViewModels;

public class WorkspaceControlsViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "WorkspaceControls";


    public List<string> FilterList { get; } = new List<string>{"Filter1", "Filter2", "Filter3"};

    public void FilterImage(object filter)
    {

    }
}