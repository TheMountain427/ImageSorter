﻿using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using DynamicData;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public ProjectConfig ProjectConfig { get; }

    //public IObservableList<string> FilterList;
    private new ObservableCollection<ImageDetails> _filterList;
    public new ObservableCollection<ImageDetails> FilterList
    {
        get { return _filterList; }
        protected set { this.RaiseAndSetIfChanged(ref _filterList, value); }
    }

    public void GoNextImage()
    {

    }

    public void GoPreviousImage()
    {

    }

    public void ResetImageView()
    {

    }

    public void FilterImage(object filter)
    {

    }
    
    public void ToggleFilterSidePane()
    {
        CurrentAppState.FilterSidePanelOpen = !CurrentAppState.FilterSidePanelOpen;
    }

    public WorkspaceControlsViewModel(ProjectConfig projectConfig, AppState appState)
    {
        this.ProjectConfig = projectConfig;
        this.CurrentAppState = appState;

        this.FilterList = this.ProjectConfig.ReferenceImages;
        
    }
}