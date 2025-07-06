using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;

namespace ImageSorter.Views;

public partial class OverlayView : ReactiveUserControl<OverlayViewModel>
{
    public OverlayView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    // Close overlay on background click
    public void CloseOnBackgroundClick(object sender, PointerPressedEventArgs eventArgs)
    {
        if (DataContext is OverlayViewModel vm)
        {
            if (vm.AllowClickOff == true)
            {
                vm.CloseOverlay.Execute(null);
            }
        }
    }
}