using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;

namespace ImageSorter.Views;

public partial class ImageOverviewView : ReactiveUserControl<ImageOverviewViewModel>
{
    public ImageOverviewView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    // These two handle toggling the SelectionModel Changed event
    // for the Flyout Listbox workaround
    public void FilterByFlyout_Opening(object sender, EventArgs eventArgs)
    {
        if (DataContext is ImageOverviewViewModel vm)
        {
            vm.ToggleSelectionModelChangedSubscription(false);
        }
    }

    public void FilterByFlyout_Opened(object sender, EventArgs eventArgs)
    {
        if (DataContext is ImageOverviewViewModel vm)
        {
            vm.ToggleSelectionModelChangedSubscription(true);
        }
    }
}