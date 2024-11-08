

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

    private ObservableCollection<ImageDetails> _referenceImages = new ObservableCollection<ImageDetails>();
    public ObservableCollection<ImageDetails> ReferenceImages
    {
        get { return _referenceImages; }
        protected set { this.RaiseAndSetIfChanged(ref _referenceImages, value); }
    }
    
    // imagine binding to a List<string> and a textbox.
    // apparently impossible
    public void AddFilter()
    {
        var shit = new ImageDetails();
        shit.FilteredValue = $"Filter{ReferenceImages.Count}";
        ReferenceImages.Add(shit);
        ProjectConfig.SetLastModifiedTime();
    }

    public void RemoveFilter()
    {
        if (ReferenceImages.Count > 0)
        {
            ReferenceImages.RemoveAt(ReferenceImages.Count - 1);
        }
        ProjectConfig.SetLastModifiedTime();
    }

    public void ApplyFilters()
    {
        // this is actually so fucking stupid
        // only way I can get both of the things to update correctly
        var tempImgRefShit = new ImageDetails[ProjectConfig.ReferenceImages.Count];
        ReferenceImages.CopyTo(tempImgRefShit,0);

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
        this.ReferenceImages = this.ProjectConfig.ReferenceImages;
    }
}