

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

namespace ImageSorter.ViewModels;

public class SortPreviewViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public override string UrlPathSegment { get; } = "SortPreview";

    public ProjectConfig ProjectConfig { get; }

    private IQueryable<ImageDetails> _sortedImageDetails { get; set; }

    public List<ImageDetails> PreviewImageDetails { get; set; }

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

    private IEnumerable<string>? _availableFilterValues;
    public IEnumerable<string>? AvailableFilterValues
    {
        get { return _availableFilterValues; }
        set { this.RaiseAndSetIfChanged(ref _availableFilterValues, value); }
    }

    // This is a list box handler, it is great
    public ISelectionModel FilterBySelectionModel { get; }

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
    
    private void FilterPreviewImagesBy(object? sender, SelectionModelSelectionChangedEventArgs e)
    {
        var filters = (IReadOnlyList<string>)e.SelectedItems;
        if (filters is not null && filters.Count() != 0)
        {
            var a = this._sortedImageDetails.Where(image => filters.Any(y => image.FilteredValue == y));
        }
        else
        {
            // Reset filters
        }
    }

    public SortPreviewViewModel(AppState AppState, ProjectConfig ProjectConfig, List<ImageDetails> SortedImageDetails, ICommand OnSuccessCommand, ICommand OnCancelCommand)
    {

        this.CurrentAppState = AppState;
        this.ProjectConfig = ProjectConfig;
        this.ContinueSortCommand = OnSuccessCommand;
        this.CancelSortCommand = OnCancelCommand;

        this._sortedImageDetails = SortImageDetailsBy(SortedImageDetails, this.ProjectConfig.ImageSortOrder).AsQueryable();

        // Set up the Filter ListBox selection handler
        this.FilterBySelectionModel = new SelectionModel<string>();
        // Allow multiple selections in the list box
        this.FilterBySelectionModel.SingleSelect = false;
        this.FilterBySelectionModel.SelectionChanged += FilterPreviewImagesBy;

        this.PreviewImageDetails = _sortedImageDetails.ToList();

        this.AvailableFilterValues = this.PreviewImageDetails.Select(x => x.FilteredValue).Distinct();

        //this.WhenAnyValue(x => x.SelectionModel.SelectedItems).Subscribe(_ => FilterPreviewImagesBy(_));

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