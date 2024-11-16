

using Avalonia.Media.Imaging;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using static ImageSorter.Models.Helpers;
using static ImageSorter.Models.Enums;
using Avalonia.Controls.Selection;
using System.Diagnostics;

namespace ImageSorter.ViewModels;

public class ImageOverviewViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public override string UrlPathSegment { get; } = "SortPreview";

    public ProjectConfig ProjectConfig { get; }

    private IQueryable<ImageDetails> _sortedImageDetails { get; set; }

    public List<ImageDetails> _previewImageDetails;
    public List<ImageDetails> PreviewImageDetails
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
    private HashSet<int> _selectedFilterIndexes { get; set; } = new HashSet<int>();

    public void ContinueSort()
    {
        ContinueSortCommand.Execute(null);
    }

    public void CancelSort()
    {
        CancelSortCommand.Execute(null);
    }


    private void _calculateMaxViewHeight(double WindowHeight)
    {
        this.MaxViewHeight = WindowHeight * 0.8;
    }

    private void _calculateMaxViewWidth(double WindowWidth)
    {
        this.MaxViewWidth = WindowWidth * 0.8;
    }
    

    // ** REFACTOR **
    private void _filterPreviewImagesBy(object? sender, SelectionModelSelectionChangedEventArgs e)
    {
        // Handle selections and deselections of items
        if (e.SelectedIndexes is not null && e.SelectedIndexes.Count != 0)
        {
            foreach (int index in e.SelectedIndexes)
            {
                this._selectedFilterIndexes.Add(index);
            }
        }

        if (e.DeselectedIndexes is not null && e.DeselectedIndexes.Count != 0)
        {
            foreach (int index in e.DeselectedIndexes)
            {
                this._selectedFilterIndexes.Remove(index);
            }
        }

        // Get the filters from their index
        if (this._selectedFilterIndexes.Count != 0 && AvailableFilterValues is not null)
        {
            var filterToSortBy = this._selectedFilterIndexes.Where(index => index >= 0 && index < AvailableFilterValues?.Count)
                                       .Select(index => AvailableFilterValues[index]).ToList();

            // Filter the view by the sorted images
            if (filterToSortBy.Count != 0)
            {
                this.PreviewImageDetails = this._sortedImageDetails.Where(image => filterToSortBy.Any(filter => filter == image.FilteredValue)).ToList();
            }
        }
    }

    // Reopening flyout removes all selections, this puts them back
    public void ReselectListValues()
    {
        Debug.WriteLine($"SelectionModel selected indexes{string.Join(',',this.FilterBy_SelectionModel.SelectedIndexes)}");
        if (this._selectedFilterIndexes.Count != 0)
        {
            foreach (var item in this._selectedFilterIndexes)
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
            this.FilterBy_SelectionModel.SelectionChanged -= _filterPreviewImagesBy;
        }
        else
        {
            this.FilterBy_SelectionModel.SelectionChanged += _filterPreviewImagesBy;
        }
    }

    public ImageOverviewViewModel(AppState AppState, ProjectConfig ProjectConfig, List<ImageDetails> SortedImageDetails, ICommand OnSuccessCommand, ICommand OnCancelCommand)
    {

        this.CurrentAppState = AppState;
        this.ProjectConfig = ProjectConfig;
        this.ContinueSortCommand = OnSuccessCommand;
        this.CancelSortCommand = OnCancelCommand;

        this._sortedImageDetails = SortImageDetailsBy(SortedImageDetails, this.ProjectConfig.ImageSortOrder).AsQueryable();

        // Set up the Filter ListBox selection handler
        this.FilterBy_SelectionModel = new SelectionModel<string>();
        // Allow multiple selections in the list box
        this.FilterBy_SelectionModel.SingleSelect = false;
        this.FilterBy_SelectionModel.SelectionChanged += _filterPreviewImagesBy;
        
        this.PreviewImageDetails = _sortedImageDetails.ToList();

        // ToList() as a workaround for https://github.com/AvaloniaUI/Avalonia/issues/12344
        this.AvailableFilterValues = this.PreviewImageDetails.Select(x => x.FilteredValue).Distinct().ToList();


        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable
                .Create(() => this.HandleDeactivation())
                .DisposeWith(disposables);


            // Dispose these cause they rely on on outside binding...?
            this.CurrentAppState.WhenAnyValue(x => x.WindowWidth).Subscribe(_ => _calculateMaxViewWidth(_)).DisposeWith(disposables);
            this.CurrentAppState.WhenAnyValue(x => x.WindowHeight).Subscribe(_ => _calculateMaxViewHeight(_)).DisposeWith(disposables);
        });
    }


    private void HandleDeactivation() { }
}