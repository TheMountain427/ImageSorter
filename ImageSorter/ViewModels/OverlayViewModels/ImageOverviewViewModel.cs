﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using Avalonia.Threading;
using ImageSorter.Models;
using ImageSorter.Models.Generators;
using ReactiveUI;
using static ImageSorter.Models.Helpers;

namespace ImageSorter.ViewModels;

public class ImageOverviewViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public override string UrlPathSegment { get; } = "SortPreview";

    public ProjectConfig ProjectConfig { get; }

    private IQueryable<ImageDetails> _sortedImageDetails { get; set; }

    // For some reason, I made it so that I have to search this for filters...
    private readonly ObservableCollection<ImageDetails> _referenceImages;

    public ObservableCollection<string> CurrentFilterValues { get; } = new();

    private bool _bulkSelectionActivated = false;
    public bool BulkSelectionActivated
    {
        get => _bulkSelectionActivated;
        set => this.RaiseAndSetIfChanged(ref _bulkSelectionActivated, value);
    }

    public IEnumerable<ImageDetails> _previewImageDetails;
    public IEnumerable<ImageDetails> PreviewImageDetails
    {
        get { return _previewImageDetails; }
        set { this.RaiseAndSetIfChanged(ref _previewImageDetails, value); }
    }

    public ICommand ContinueSortCommand { get; }

    public ICommand CancelSortCommand { get; }

    private double _maxViewHeight;
    public double MaxViewHeight
    {
        get { return _maxViewHeight; }
        set { this.RaiseAndSetIfChanged(ref _maxViewHeight, value); }
    }

    private double _maxViewWidth;
    public double MaxViewWidth
    {
        get { return _maxViewWidth; }
        set { this.RaiseAndSetIfChanged(ref _maxViewWidth, value); }
    }

    private List<string>? _availableFilterValues;
    public List<string>? AvailableFilterValues
    {
        get { return _availableFilterValues; }
        set { this.RaiseAndSetIfChanged(ref _availableFilterValues, value); }
    }

    // This is a list box handler, it is great
    // Nvm it's clunky when used in a flyout
    public ISelectionModel FilterBy_SelectionModel { get; }

    // Store selected items from filter list. Workaround since Flyout deselects its selections when it reopens
    // Additonally, the SelectionModel forgets what was selected so we have to store them somewhere
    // This is the true sorce of what is actually selected
    private HashSet<int> _filterByIndexesSelected { get; set; } = new HashSet<int>();

    public ImgOrderOptions ImageOrderOptions { get; }

    private ImgOrderOption _imageSortOrder;
    public ImgOrderOption ImageSortOrder
    {
        get { return _imageSortOrder; }
        protected set { this.RaiseAndSetIfChanged(ref _imageSortOrder, value); }
    }

    public void ContinueSort()
    {
        ContinueSortCommand.Execute(null);
    }

    public void CancelSort()
    {
        CancelSortCommand.Execute(null);
    }


    private void CalculateMaxViewHeight(double WindowHeight)
    {
        this.MaxViewHeight = WindowHeight * 0.8;
    }

    private void CalculateMaxViewWidth(double WindowWidth)
    {
        this.MaxViewWidth = WindowWidth * 0.8;
    }

    // ** REFACTOR **
    private void FilterPreviewImagesBy(object? sender, SelectionModelSelectionChangedEventArgs e)
    {
        // Handle selections and deselections of items
        if (e.SelectedIndexes is not null && e.SelectedIndexes.Count != 0)
        {
            foreach (int index in e.SelectedIndexes)
            {
                this._filterByIndexesSelected.Add(index);
            }
        }

        if (e.DeselectedIndexes is not null && e.DeselectedIndexes.Count != 0)
        {
            foreach (int index in e.DeselectedIndexes)
            {
                this._filterByIndexesSelected.Remove(index);
            }
        }

        FilterImagesByValues();
    }

    private void FilterImagesByValues()
    {
        // Get the filters from their index
        if (this._filterByIndexesSelected.Count != 0 && AvailableFilterValues is not null)
        {
            var filterToSortBy = this._filterByIndexesSelected.Where(index => index >= 0 && index < AvailableFilterValues?.Count)
                                       .Select(index => AvailableFilterValues[index]);

            // Filter the view by the sorted images
            if (filterToSortBy.Any())
            {
                //this.PreviewImageDetails = this._sortedImageDetails.Where(image => filterToSortBy.Any(filter => filter == image.FilteredValue)).ToList();
                this.PreviewImageDetails = SortImageDetailsQueryableBy(this._sortedImageDetails, this.ImageSortOrder.OptionEnum)
                                                .Where(image => filterToSortBy.Any(filter => filter == image.FilteredValue));
            }
        }
        else
        {
            this.PreviewImageDetails = SortImageDetailsQueryableBy(this._sortedImageDetails, this.ImageSortOrder.OptionEnum);
        }

        // Update view so images get redrawn in new order
        this.RaisePropertyChanged(nameof(PreviewImageContainers));
    }

    private void UpdateImageListOrder()
    {
        // Just calling this because what we really want is SortImageDetailsQueryableBy(this._sortedImageDetails, this.ImageSortOrder.OptionEnum)
        // But we need to handle if filters are selected. If we didn't call this, we would just have to basically copy that method here
        FilterImagesByValues();
    }

    // Reopening flyout removes all selections, this puts them back
    public void ReselectListValues()
    {
        Debug.WriteLine($"SelectionModel selected indexes{string.Join(',', this.FilterBy_SelectionModel.SelectedIndexes)}");
        if (this._filterByIndexesSelected.Count != 0)
        {
            foreach (var item in this._filterByIndexesSelected)
            {
                this.FilterBy_SelectionModel.Select(item);
            }
        }
    }

    // Flyout sends a deselect all when re-opening. This skips those events
    public void ToggleSelectionModelChangedSubscription(bool value)
    {
        if (value == false)
        {
            this.FilterBy_SelectionModel.SelectionChanged -= FilterPreviewImagesBy;
        }
        else
        {
            this.FilterBy_SelectionModel.SelectionChanged += FilterPreviewImagesBy;
        }
    }

    public ICommand? ImageClickCommand { get; }

    // Switched over to a VirtualizingStackPanel. Since there is no VirtualizingWrapPanel,
    // we have to create the 'wrapped rows' our selves.
    // Also switch over to lazy loading with yield so we don't hand on large collections
    public IEnumerable<StackPanel> PreviewImageContainers => CreateImageRows();
    public IEnumerable<StackPanel> CreateImageRows()
    {
        var numItemsInRow = (int)Math.Floor(MaxViewWidth / 200);
        numItemsInRow = numItemsInRow == 0 ? 1 : numItemsInRow;

        var previewRows = ImageOverviewContainerGenerator.CreateImageRows(
            PreviewImageDetails,
            numItemsInRow == 0 ? 1 : numItemsInRow,
            this.WhenAnyValue(x => x.BulkSelectionActivated),
            img => { ImageClickCommand!.Execute(img.FileName); },
            ReactiveCommand.Create<(Button btn, ImageDetails img)>(t => OnBulkImageButtonClicked(t.btn, t.img))
        );

        foreach (var row in previewRows)
        {
            yield return row;
        }
    }

    private HashSet<ImageDetails> BulkSelectedImages = new();

    public void OnBulkFilterClicked(string filterName)
    {
        foreach (var image in BulkSelectedImages)
        {
            image.FilteredValue = filterName;
        }

        BulkSelectionActivated = false;
        this.RaisePropertyChanged(nameof(PreviewImageContainers));
    }

    private void OnBulkImageButtonClicked(Button btn, ImageDetails img)
    {
        if (btn.Tag?.ToString() == "1")
        {
            BulkSelectedImages.Add(img);
        }
        else
        {
            BulkSelectedImages.Remove(img);
        }
    }

    public ImageOverviewViewModel(AppState CurrentAppState, ProjectConfig ProjectConfig, List<ImageDetails> SortedImageDetails,
                                  ICommand OnSuccessCommand, ICommand OnCancelCommand, ICommand? ImageClickCommand,
                                  ICommand ChangeSortOrder, ImgOrderOptions ImageOrderOptions, ImgOrderOption ImageSortOrder,
                                  ObservableCollection<ImageDetails> ReferenceImages) : base(CurrentAppState)
    {
        this.ProjectConfig = ProjectConfig;
        this.ContinueSortCommand = OnSuccessCommand;
        this.CancelSortCommand = OnCancelCommand;
        this.ImageOrderOptions = ImageOrderOptions;
        this.ImageSortOrder = ImageSortOrder;

        _referenceImages = ReferenceImages;
        _referenceImages.Select(x => x.FilteredValue).Distinct().ToList().ForEach(CurrentFilterValues.Add);
        _referenceImages.CollectionChanged += (_, __) =>
        {
            CurrentFilterValues.Clear();
            _referenceImages.Select(x => x.FilteredValue).Distinct().ToList().ForEach(CurrentFilterValues.Add);
        };

        this.WhenAnyValue(x => x.BulkSelectedImages).Subscribe(_ => BulkSelectedImages.Clear());

        this._sortedImageDetails = SortImageDetailsBy(SortedImageDetails, this.ImageSortOrder.OptionEnum).AsQueryable();

        this.ImageClickCommand = ImageClickCommand;

        // Set up the Filter ListBox selection handler
        this.FilterBy_SelectionModel = new SelectionModel<string>
        {
            // Allow multiple selections in the list box
            SingleSelect = false
        };
        this.FilterBy_SelectionModel.SelectionChanged += FilterPreviewImagesBy;

        this.PreviewImageDetails = _sortedImageDetails;

        // ToList() as a workaround for https://github.com/AvaloniaUI/Avalonia/issues/12344
        this.AvailableFilterValues = this.PreviewImageDetails.Select(x => x.FilteredValue).Distinct().ToList();

        this.WhenAnyValue(x => x.ImageSortOrder).Subscribe(_ => ChangeSortOrder.Execute(_));

        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable
                .Create(() => this.HandleDeactivation())
                .DisposeWith(disposables);


            // Dispose these cause they rely on on outside binding...?
            this.CurrentAppState.WhenAnyValue(x => x.WindowWidth).Subscribe(_ => CalculateMaxViewWidth(_)).DisposeWith(disposables);
            this.CurrentAppState.WhenAnyValue(x => x.WindowHeight).Subscribe(_ => CalculateMaxViewHeight(_)).DisposeWith(disposables);

            // I guess I am supposed to actually put all of these here
            // Putting it here does not call it on ViewModel construction
            this.WhenAnyValue(x => x.ImageSortOrder).Subscribe(_ => UpdateImageListOrder());
        });
    }

    private void HandleDeactivation() { }
}