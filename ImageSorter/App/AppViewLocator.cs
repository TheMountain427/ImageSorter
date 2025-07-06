using ImageSorter.ViewModels;
using ImageSorter.Views;
using ReactiveUI;

namespace ImageSorter
{
    public class AppViewLocator : IViewLocator
    {
        public IViewFor ResolveView<T>(T viewModel, string contract = null) => viewModel switch
        {
            ProjectSelectionViewModel context => new ProjectSelectionView { DataContext = context },
            WorkspaceViewModel context => new WorkspaceView { DataContext = context },
            CurrentImageViewModel context => new CurrentImageView { DataContext = context },
            WorkspaceControlsViewModel context => new WorkspaceControlsView { DataContext = context },
            WorkspaceThumbnailViewModel context => new WorkspaceThumbnailView { DataContext = context },
            WorkspaceReferenceImageViewModel context => new WorkspaceReferenceImageView { DataContext = context },
            WorkspaceFilterViewModel context => new WorkspaceFilterView { DataContext = context },
            OverlayViewModel context => new OverlayView { DataContext = context },
            SortConfirmationViewModel context => new SortConfirmationView { DataContext = context },
            ImageOverviewViewModel context => new ImageOverviewView { DataContext = context },
            InProgressViewModel context => new InProgressView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}