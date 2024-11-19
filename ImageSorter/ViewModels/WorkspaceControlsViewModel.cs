using Avalonia.Controls;
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
    private ObservableCollection<ImageDetails> _referenceImages = new ObservableCollection<ImageDetails>();
    public ObservableCollection<ImageDetails> ReferenceImages
    {
        get { return _referenceImages; }
        protected set { this.RaiseAndSetIfChanged(ref _referenceImages, value); }
    }

    public double ControlsContainerWidth { get; set; }

    public ICommand GoNextImage { get; }

    public ICommand GoPreviousImage { get; }

    public ICommand GoFirstImage { get; }

    public ICommand GoLastImage { get; }

    public ICommand ResetImagePosition { get; }

    public ICommand SetImageFilteredValue { get; }

    public ICommand DebugCommand { get; }
    
    public ImgOrderOptions ImageOrderOptions { get; set; }

    public HorizontalToggles ThumbnailHorizontalSettings { get; set; } = new HorizontalToggles();

    public VerticalToggles ThumbnailVerticalSettings { get; set; } = new VerticalToggles();

    public HorizontalToggles ControlsHorizontalSettings { get; set; } = new HorizontalToggles();

    public VerticalToggles ControlsVerticalSettings { get; set; } = new VerticalToggles();

    private ImgOrderOption _imageSortOrder;
    public ImgOrderOption ImageSortOrder
    {
        get { return _imageSortOrder; }
        set { this.RaiseAndSetIfChanged(ref _imageSortOrder, value); }
    }
    private bool _advanceSettingsOpen;
    public bool AdvancedSettingsOpen
    {
        get { return _advanceSettingsOpen; }
        set { this.RaiseAndSetIfChanged(ref _advanceSettingsOpen, value); }
    }

    public void ToggleFilterSidePane()
    {
        CurrentAppState.FilterSidePanelOpen = !CurrentAppState.FilterSidePanelOpen;
    }

    public ICommand BeginSortCommand { get; }

    public ICommand OpenAdditonalsViewThing { get; }

    public WorkspaceControlsViewModel(AppState CurrentAppState, ProjectConfig ProjectConfig, ImageCommands ImageCommands) : base (CurrentAppState)
    {
        this.ProjectConfig = ProjectConfig;
        this.ImageCommands = ImageCommands;

        this.ReferenceImages = this.ProjectConfig.ReferenceImages;

        this.GoNextImage = ImageCommands.NavigateNextMainImage;
        this.GoPreviousImage = ImageCommands.NavigatePreviousMainImage;
        this.ResetImagePosition = ImageCommands.ResetMainImagePosition;
        this.SetImageFilteredValue = ImageCommands.SetImageFilteredValue;
        this.GoFirstImage = ImageCommands.NavigateFirstImage;
        this.GoLastImage = ImageCommands.NavigateLastImage;
        this.BeginSortCommand = ImageCommands.BeginImageSorting;
        this.ImageOrderOptions = ImageCommands.ImageOrderOptions;
        this.ImageSortOrder = ImageCommands.ImageSortOrder;

        this.WhenAnyValue(x => x.ImageSortOrder).Subscribe(_ => ImageCommands.ChangeImageSortOrder.Execute(_));

        this.OpenAdditonalsViewThing = ReactiveCommand.Create(() => AdvancedSettingsOpen = !AdvancedSettingsOpen);
        this.DebugCommand = ReactiveCommand.Create(() => AdvancedSettingsOpen = !AdvancedSettingsOpen);

       
        this.ThumbnailHorizontalSettings.SetSelectedOption(this.CurrentAppState.ThumbnailHorizontalAlign);
        this.ThumbnailHorizontalSettings.WhenAnyValue(x => x.SelectedOption).Subscribe(_ => this.CurrentAppState.ThumbnailHorizontalAlign = _);
       
        this.ThumbnailVerticalSettings.SetSelectedOption(this.CurrentAppState.ThumbnailVerticalAlign);
        this.ThumbnailVerticalSettings.WhenAnyValue(x => x.SelectedOption).Subscribe(_ => this.CurrentAppState.ThumbnailVerticalAlign = _);
       
        this.ControlsHorizontalSettings.SetSelectedOption(this.CurrentAppState.ControlsHorizontalAlign);
        this.ControlsHorizontalSettings.WhenAnyValue(x => x.SelectedOption).Subscribe(_ => this.CurrentAppState.ControlsHorizontalAlign = _);
       
        this.ControlsVerticalSettings.SetSelectedOption(this.CurrentAppState.ControlsVerticalAlign);
        this.ControlsVerticalSettings.WhenAnyValue(x => x.SelectedOption).Subscribe(_ => this.CurrentAppState.ControlsVerticalAlign = _);
    }
}