using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using DynamicData;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
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

    public ReferenceSplitToggles ReferenceSplitSettings { get; set; } = new ReferenceSplitToggles();

    private ReversibleStackPanel? RStackPanel { get; set; }

    private bool _reverseControlsOrder = false;
    public bool ReverseControlsOrder
    {
        get { return _reverseControlsOrder; }
        protected set { this.RaiseAndSetIfChanged(ref _reverseControlsOrder, value); }
    }

    private ImgOrderOption _imageSortOrder;
    public ImgOrderOption ImageSortOrder
    {
        // I think I am supposed to be setting _imageSortOrder = <shit>
        // not ImageSortOrder = <shit>. Oh well
        get { return _imageSortOrder; }
        [MemberNotNull(nameof(_imageSortOrder))]
        set { this.RaiseAndSetIfChanged(ref _imageSortOrder!, value); }
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

    private void HandleStackPanelOrder()
    {
        switch (this.CurrentAppState.ControlsVerticalAlign)
        {
            case VerticalAlignment.Top:
                if (this.ReverseControlsOrder == false)
                {
                    this.ReverseControlsOrder = true;
                    this.RStackPanel?.InvalidateArrange(); // Force update layout
                }
                break;
            default:
                if (this.ReverseControlsOrder == true)
                {
                    this.ReverseControlsOrder = false;
                    this.RStackPanel?.InvalidateArrange();
                }
                break;
        }
    }

    public void SetReversibleStackPanel(ReversibleStackPanel? rStack)
    {
        if (rStack is not null)
        {
            this.RStackPanel = rStack;
        }
    }

    public ICommand BeginSortCommand { get; }

    public ICommand OpenAdditonalsViewThing { get; }

    public ICommand BrowseNewOutputDir { get; }

    public ICommand OpenOverviewCommand { get; }

    public WorkspaceControlsViewModel(AppState CurrentAppState, ProjectConfig ProjectConfig, ImageCommands ImageCommands) : base(CurrentAppState)
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
        this.BrowseNewOutputDir = ImageCommands.BrowseForNewOutput;
        this.OpenOverviewCommand = ImageCommands.OpenOverview;

        this.WhenAnyValue(x => x.ImageSortOrder).Subscribe(_ => ImageCommands.ChangeImageSortOrder.Execute(_));

        // Open the additional settings view
        this.OpenAdditonalsViewThing = ReactiveCommand.Create(() => AdvancedSettingsOpen = !AdvancedSettingsOpen);
        this.DebugCommand = ReactiveCommand.Create(() => AdvancedSettingsOpen = !AdvancedSettingsOpen);

        // Set up commands for settings that change controls and thumbnail viewer position
        // Positions are set through CurrentAppState
        this.ThumbnailHorizontalSettings.SetSelectedOption(this.CurrentAppState.ThumbnailHorizontalAlign);
        this.ThumbnailHorizontalSettings.WhenAnyValue(x => x.SelectedOption).Subscribe(_ => this.CurrentAppState.ThumbnailHorizontalAlign = _);

        this.ThumbnailVerticalSettings.SetSelectedOption(this.CurrentAppState.ThumbnailVerticalAlign);
        this.ThumbnailVerticalSettings.WhenAnyValue(x => x.SelectedOption).Subscribe(_ => this.CurrentAppState.ThumbnailVerticalAlign = _);

        this.ControlsHorizontalSettings.SetSelectedOption(this.CurrentAppState.ControlsHorizontalAlign);
        this.ControlsHorizontalSettings.WhenAnyValue(x => x.SelectedOption).Subscribe(_ => this.CurrentAppState.ControlsHorizontalAlign = _);

        this.ControlsVerticalSettings.SetSelectedOption(this.CurrentAppState.ControlsVerticalAlign);
        this.ControlsVerticalSettings.WhenAnyValue(x => x.SelectedOption).Subscribe(_ => this.CurrentAppState.ControlsVerticalAlign = _);

        // Set up commands for how reference images are split between reference views
        this.ReferenceSplitSettings.SetSelectedOption(this.CurrentAppState.ReferenceSplitSetting);
        this.ReferenceSplitSettings.WhenAnyValue(x => x.SelectedOption).Subscribe(_ => this.CurrentAppState.ReferenceSplitSetting = _);

        // Reverse controls if VerticalAlignment == top
        this.HandleStackPanelOrder();
        this.CurrentAppState.WhenAnyValue(x => x.ControlsVerticalAlign).Subscribe(_ => HandleStackPanelOrder());
    }
}