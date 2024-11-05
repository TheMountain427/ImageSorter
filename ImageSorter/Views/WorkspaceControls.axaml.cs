using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;

namespace ImageSorter.Views;

public partial class WorkspaceControls : ReactiveUserControl<WorkspaceControlsViewModel>
{
    public WorkspaceControls()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}