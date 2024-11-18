using Avalonia.Data.Converters;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class WorkspaceThumbnailViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public override string UrlPathSegment { get; } = "WorkspaceThumbnailView";

    public ImageDetails EmptyImageDetails { get; } = new ImageDetails()
    {
        FileName = "",
        FilePath = "",
        FilteredValue = ""
    };

    private ImageDetails _previous_2_ImageDetails;
    public ImageDetails Previous_2_ImageDetails
    {
        get { return _previous_2_ImageDetails; }
        set { this.RaiseAndSetIfChanged(ref _previous_2_ImageDetails, value); }
    }

    private ImageDetails _previous_1_ImageDetails;
    public ImageDetails Previous_1_ImageDetails
    {
        get { return _previous_1_ImageDetails; }
        set { this.RaiseAndSetIfChanged(ref _previous_1_ImageDetails, value); }
    }

    private ImageDetails _current_ImageDetails;
    public ImageDetails Current_ImageDetails
    {
        get { return _current_ImageDetails; }
        set { this.RaiseAndSetIfChanged(ref _current_ImageDetails, value); }
    }

    private ImageDetails _next_1_ImageDetails;
    public ImageDetails Next_1_ImageDetails
    {
        get { return _next_1_ImageDetails; }
        set { this.RaiseAndSetIfChanged(ref _next_1_ImageDetails, value); }
    }

    private ImageDetails _next_2_ImageDetails;
    public ImageDetails Next_2_ImageDetails
    {
        get { return _next_2_ImageDetails; }
        set { this.RaiseAndSetIfChanged(ref _next_2_ImageDetails, value); }
    }

    private List<ImageDetails> _sortedImageDetails { get; set; }

    private int _currentIndex;
    public int CurrentIndex
    {
        get { return _currentIndex; }
        set { this.RaiseAndSetIfChanged(ref _currentIndex, value); }
    }

    private int _currentMax;
    public int CurrentMax
    {
        get { return _currentMax; }
        set { this.RaiseAndSetIfChanged(ref _currentMax, value); }
    }

    // Lol, lmao even
    [MemberNotNull(nameof(_previous_2_ImageDetails))]
    [MemberNotNull(nameof(_previous_1_ImageDetails))]
    [MemberNotNull(nameof(_current_ImageDetails))]
    [MemberNotNull(nameof(_next_1_ImageDetails))]
    [MemberNotNull(nameof(_next_2_ImageDetails))]
    private void SetupThumbnails(List<ImageDetails> SortedImageDetails, int CurrentImageIndex)
    {
        this.CurrentIndex = CurrentImageIndex;

        // uh I tried to make it not complex, don't know about that...
        var currentIndex = CurrentImageIndex;
        var maxIndex = SortedImageDetails.Count - 1;

        // ex: 101 images > max index = 100
        switch (currentIndex)
        {
            case 0: // At the first, don't load both
                this.Previous_2_ImageDetails = this.EmptyImageDetails;
                this.Previous_1_ImageDetails = this.EmptyImageDetails;
                break;
            case 1: // At the second image, can load one back
                this.Previous_2_ImageDetails = this.EmptyImageDetails;
                this.Previous_1_ImageDetails = SortedImageDetails[CurrentImageIndex - 1];
                break;
            default: // All other options means we can load both... unless we have only 1 or two images in the inputs
                this.Previous_2_ImageDetails = maxIndex >= 1 ? SortedImageDetails[CurrentImageIndex - 2] : this.EmptyImageDetails;
                this.Previous_1_ImageDetails = maxIndex >= 0 ? SortedImageDetails[CurrentImageIndex - 1] : this.EmptyImageDetails;
                break;
        }

        // Don't think I could switch this, just checks if the index is actually valid
        if (currentIndex < 0 || currentIndex > maxIndex)
        {
            this.Current_ImageDetails = this.EmptyImageDetails;
        }
        else
        {
            this.Current_ImageDetails = SortedImageDetails[CurrentImageIndex];
        }

        // ex: 101 images > max index = 100
        switch (maxIndex - currentIndex)
        {
            case > 1: // At most at index 98 out of 100, so can load 2 ahead
                this.Next_2_ImageDetails = SortedImageDetails[CurrentImageIndex + 2];
                this.Next_1_ImageDetails = SortedImageDetails[CurrentImageIndex + 1];
                break;
            case 1: // At index 99 out of 100, so can only load 1 ahead
                this.Next_2_ImageDetails = this.EmptyImageDetails;
                this.Next_1_ImageDetails = SortedImageDetails[CurrentImageIndex + 1];
                break;
            default: // Everything else
                this.Next_2_ImageDetails = this.EmptyImageDetails;
                this.Next_1_ImageDetails = this.EmptyImageDetails;
                break;
        }

    }

    private void UpdateSortOrder(List<ImageDetails> SortedImageDetials)
    {
        this.CurrentMax = SortedImageDetials.Count;
        this._sortedImageDetails = SortedImageDetials;
    }

    private ICommand ImageShiftCommand { get; }

    public void ShiftImage(string value)
    {
        if (int.TryParse(value, out int shift))
        {
            this.ImageShiftCommand.Execute(shift);
        }
    }


    public WorkspaceThumbnailViewModel(AppState CurrentAppState, int CurrentImageIndex, List<ImageDetails> SortedImageDetails,
                                       IObservable<int> CurrentImageIndexObservable, IObservable<List<ImageDetails>> SortedImageDetialsObservable,
                                       ICommand ImageShiftCommand) : base (CurrentAppState)
    {
        this._sortedImageDetails = SortedImageDetails;
        this.ImageShiftCommand = ImageShiftCommand;
        // Set up thumbnails

        this.SetupThumbnails(SortedImageDetails, CurrentImageIndex);

        CurrentImageIndexObservable.Subscribe(_ => SetupThumbnails(this._sortedImageDetails, _));

        SortedImageDetialsObservable.Subscribe(_ => UpdateSortOrder(_));


        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable
                .Create(() => this.HandleDeactivation())
                .DisposeWith(disposables);
        });
    }

    private void HandleDeactivation() { }
}

