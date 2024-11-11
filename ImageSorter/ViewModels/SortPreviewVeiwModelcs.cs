

using ImageSorter.Models;
using ReactiveUI;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class SortPreviewViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "SortPreview";

    public ProjectConfig ProjectConfig { get; }

    public ICommand ContinueSortCommand { get; }

    public ICommand CancelSortCommand { get; }

    public void ContinueSort()
    {
        ContinueSortCommand.Execute(null);
    }

    public void CancelSort()
    {
        ContinueSortCommand.Execute(null);
    }

    public SortPreviewViewModel(AppState AppState, ProjectConfig ProjectConfig, ICommand OnSuccessCommand, ICommand OnCancelCommand)
    {
        this.CurrentAppState = AppState;
        this.ProjectConfig = ProjectConfig;
        this.ContinueSortCommand = OnSuccessCommand;
        this.CancelSortCommand = OnCancelCommand;
    }
}