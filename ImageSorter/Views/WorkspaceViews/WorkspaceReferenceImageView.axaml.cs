using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;

namespace ImageSorter.Views;

public partial class WorkspaceReferenceImageView : ReactiveUserControl<WorkspaceReferenceImageViewModel>
{
    public WorkspaceReferenceImageView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    // I don't care to try and get this to work in the vm
    // I made my bed with needing to SetLastModifiedTime to trigger JSON writes on collection change
    // and I fully intend to lie in it.
    public void FilterNameChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is WorkspaceReferenceImageViewModel vm)
        {
            vm.ProjectConfig.SetLastModifiedTime();
        }
    }
}