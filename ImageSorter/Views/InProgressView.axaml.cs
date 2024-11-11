using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;

namespace ImageSorter.Views;

public partial class InProgressView : ReactiveUserControl<InProgressViewModel>
{
    public InProgressView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}