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

    private ImageCommands ImageCommands { get; }

    // Probably should be read only
    private ObservableCollection<ImageDetails> _referenceImages;
    public ObservableCollection<ImageDetails> ReferenceImages
    {
        get { return _referenceImages; }
        protected set { this.RaiseAndSetIfChanged(ref _referenceImages, value); }
    }

    public ICommand GoNextImage { get; }

    public ICommand GoPreviousImage { get; }

    public ICommand ResetImagePosition { get; }

    public void FilterImage(object filter)
    {

    }

    public void ToggleFilterSidePane()
    {
        CurrentAppState.FilterSidePanelOpen = !CurrentAppState.FilterSidePanelOpen;
    }

    public WorkspaceControlsViewModel(ProjectConfig projectConfig, AppState appState, ImageCommands imageCommands)
    {
        this.ProjectConfig = projectConfig;
        this.CurrentAppState = appState;
        this.ImageCommands = imageCommands;

        this.ReferenceImages = this.ProjectConfig.ReferenceImages;

        this.GoNextImage = ImageCommands.NavigateNextMainImage;
        this.GoPreviousImage = ImageCommands.NavigatePreviousMainImage;
        this.ResetImagePosition = ImageCommands.ResetMainImagePosition;

    }
}