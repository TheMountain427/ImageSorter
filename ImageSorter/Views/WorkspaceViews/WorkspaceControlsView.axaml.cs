using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageSorter.ViewModels;
using ReactiveUI;

namespace ImageSorter.Views;

public partial class WorkspaceControlsView : ReactiveUserControl<WorkspaceControlsViewModel>
{
    private readonly ReversibleStackPanel? _rStackPanel;

    private void AttachRStackToVM(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (_rStackPanel is not null && _rStackPanel.DataContext is WorkspaceControlsViewModel vm)
        {
            vm.SetReversibleStackPanel(_rStackPanel);
            _rStackPanel.AttachedToLogicalTree -= AttachRStackToVM;

        }
    }
    public WorkspaceControlsView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);

        _rStackPanel = this.FindControl<ReversibleStackPanel>("RStackPanel");

        if (_rStackPanel is not null)
        {
            _rStackPanel.AttachedToLogicalTree += AttachRStackToVM;
        }
    }
}