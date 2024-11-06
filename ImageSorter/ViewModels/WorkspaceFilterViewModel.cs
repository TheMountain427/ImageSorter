

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

    private new ObservableCollection<ImageDetails> _filters = new ObservableCollection<ImageDetails>();
    public new ObservableCollection<ImageDetails> Filters
    {
        get { return _filters; }
        protected set { this.RaiseAndSetIfChanged(ref _filters, value); }
    }
    
    // imagine binding to a List<string> and a textbox.
    // apparently impossible
    public void AddFilter()
    {
        var shit = new ImageDetails();
        shit.FilteredValue = $"Filter{Filters.Count}";
        Filters.Add(shit);
        ProjectConfig.SetLastModifiedTime();
    }

    public void RemoveFilter()
    {
        if (Filters.Count > 0)
        {
            Filters.RemoveAt(Filters.Count - 1);
        }
        ProjectConfig.SetLastModifiedTime();
    }

    public void ApplyFilters()
    {
        // this is actually so fucking stupid
        // only way I can get both of the things to update correctly
        var tempImgRefShit = new ImageDetails[ProjectConfig.ReferenceImages.Count];
        Filters.CopyTo(tempImgRefShit,0);
        ProjectConfig.ReferenceImages.Clear();
        if (tempImgRefShit.Count() > 0)
        {
            foreach (var fuckingshit in tempImgRefShit)
            {
                this.ProjectConfig.ReferenceImages.Add(fuckingshit);
            }
        }
        ProjectConfig.SetLastModifiedTime();
    }

    public WorkspaceFilterViewModel(ProjectConfig projectConfig)
    {
        this.ProjectConfig = projectConfig;
        this.Filters = this.ProjectConfig.ReferenceImages;
    }
}