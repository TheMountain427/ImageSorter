

using ImageSorter.Models;
using System.Collections.Generic;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class SortConfirmationViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "SortConfirmationViewModel";

    public List<SortConfirmation> SortConfirmations { get; }

    public bool SelectionSet { get; }

    public ICommand CancelSort { get; }

    public void ConfirmSelection()
    {

    }

    public SortConfirmationViewModel(IEnumerable<SortConfirmation> sortConfirmations, ICommand CloseOverlay)
    {
        this.SortConfirmations = (List<SortConfirmation>)sortConfirmations;
        this.CancelSort = CloseOverlay;
    }
}