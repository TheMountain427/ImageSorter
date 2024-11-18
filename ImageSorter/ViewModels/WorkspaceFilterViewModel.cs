

using DynamicData;
using ImageSorter.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace ImageSorter.ViewModels;

public class WorkspaceFilterViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; }
    public ProjectConfig ProjectConfig { get; }

    private ObservableCollection<ImageDetails> _referenceImages;
    public ObservableCollection<ImageDetails> ReferenceImages
    {
        get { return _referenceImages; }
        protected set { this.RaiseAndSetIfChanged(ref _referenceImages, value); }
    }
    
    public void AddFilter()
    {

        var newRefImg = new ImageDetails()
        {
            FilteredValue =  $"Filter{ReferenceImages.Count}"
        };

        ReferenceImages.Add(newRefImg);
    }

    public void RemoveFilter()
    {
        if (ReferenceImages.Count > 0)
        {
            ReferenceImages.RemoveAt(ReferenceImages.Count - 1);
        }
    }

    public void CloseFilterPanel()
    {
        this.CurrentAppState.FilterSidePanelOpen = !this.CurrentAppState.FilterSidePanelOpen;
    }

    public WorkspaceFilterViewModel(AppState CurrentAppState, ProjectConfig projectConfig) : base (CurrentAppState)
    {
        this.CurrentAppState = CurrentAppState;
        this.ProjectConfig = projectConfig;
        this.ReferenceImages = this.ProjectConfig.ReferenceImages;
    }
}