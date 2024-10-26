using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;

namespace ImageSorter.Views;

public partial class WorkspaceView : ReactiveUserControl<WorkspaceViewModel>
{
    public WorkspaceView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}
