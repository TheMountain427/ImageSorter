using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;
using System;

namespace ImageSorter.Views;

public partial class ImageOverviewView : ReactiveUserControl<ImageOverviewViewModel>
{
    public ImageOverviewView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    public void Flyout_Opened(object sender, EventArgs eventArgs)
    {
        if (DataContext is ImageOverviewViewModel vm)
        {
            vm.ToggleSelectionModelChangedSubscription(true);
        }
    }

    public void Flyout_Closed(object sender, EventArgs eventArgs)
    {
        if (DataContext is ImageOverviewViewModel vm)
        {
            vm.ToggleSelectionModelChangedSubscription(false);
        }

    }
}