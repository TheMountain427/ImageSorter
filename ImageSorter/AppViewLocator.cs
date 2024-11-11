using ImageSorter.ViewModels;
using ImageSorter.Views;
using ReactiveUI;
using System;
using System.Linq;

namespace ImageSorter
{
    public class AppViewLocator : IViewLocator
    {
        //public IViewFor ResolveView(RoutingState router,  string UrlPathSegment)
        //{
        //    var a = router.NavigationStack.Where(x => x.UrlPathSegment == UrlPathSegment);
        //    return new CurrentImageView{DataContext = a};
        //}

        //public IViewFor ResolveView<T>(T viewModel, CurrentImageView imgView, string contract = null)
        //{
        //    return imgView;
        //}

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
            DebugViewModel context => new DebugView { DataContext = context },
            SortConfirmationViewModel context => new SortConfirmationView { DataContext = context },
            SortPreviewViewModel context => new SortPreviewView { DataContext = context },
            InProgressViewModel context => new InProgressView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}