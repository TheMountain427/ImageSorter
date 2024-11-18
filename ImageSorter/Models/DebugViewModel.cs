

using ImageSorter.Models;

namespace ImageSorter.ViewModels;

public class DebugViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "DebugViewModel";

    public DebugViewModel(AppState CurrentAppState) : base (CurrentAppState)
    {

    }
}