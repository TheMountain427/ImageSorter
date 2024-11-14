

using Avalonia.Media.Imaging;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class SortPreviewViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public override string UrlPathSegment { get; } = "SortPreview";

    public ProjectConfig ProjectConfig { get; }

    public List<ImageDetails> SortedImageDetails { get; set; }

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
    
    private void HandleDeactivation() { }

    public SortPreviewViewModel(AppState AppState, ProjectConfig ProjectConfig, List<ImageDetails> SortedImageDetails, ICommand OnSuccessCommand, ICommand OnCancelCommand)
    {

        this.CurrentAppState = AppState;
        this.ProjectConfig = ProjectConfig;
        this.ContinueSortCommand = OnSuccessCommand;
        this.CancelSortCommand = OnCancelCommand;

        this.SortedImageDetails = SortedImageDetails;

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
}