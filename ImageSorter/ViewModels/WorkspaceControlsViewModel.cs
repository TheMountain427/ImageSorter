using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using static ImageSorter.Models.Helpers;

namespace ImageSorter.ViewModels;

public class WorkspaceControlsViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "WorkspaceControls";

    public ProjectConfig ProjectConfig { get; }

    public List<string> FilterList { get; protected set; } = new List<string> { "Filter1", "Filter2", "Filter3" };

    public List<string> _tempFilterList;
    public List<string> TempFilterList 
    {
        get { return _tempFilterList; }
        set
        {
            this.RaiseAndSetIfChanged(ref _tempFilterList, value);
        }
    }
     

    private decimal _filterAmount;
    public decimal FilterAmount
    {
        get { return _filterAmount; }
        set
        {
            this.RaiseAndSetIfChanged(ref _filterAmount, value);
            //_onFilterAmountChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private EventHandler _onFilterAmountChanged;
    public event EventHandler OnFilterAmountChanged
    {
        add
        {
            _onFilterAmountChanged += value;
        }
        remove
        {
            _onFilterAmountChanged -= value;
        }
    }

    private void ChangeFilterAmount()
    {
        if (TempFilterList.Count < FilterAmount)
        {
            //var toAdd = FilterAmount - TempFilterList.Count;
            while (TempFilterList.Count < FilterAmount)
            {
                TempFilterList.Add($"Filter{TempFilterList.Count}");
            }
        }
        else if (TempFilterList.Count > FilterAmount)
        {
            var toRemove = TempFilterList.Count - (int)FilterAmount;
            TempFilterList.RemoveRange((int)FilterAmount , toRemove);

        }
    }

    public void SetFilters()
    {
        FilterList.Clear();
        FilterList.AddRange(TempFilterList);
        ProjectConfig.SetLastModifiedTime();
    }
    public void GoNextImage()
    {

    }

    public void GoPreviousImage()
    {

    }

    public void SetFilterValues()
    {

    }

    public void ResetImageView()
    {

    }

    public void FilterImage(object filter)
    {

    }

    public WorkspaceControlsViewModel(ProjectConfig projectConfig)
    {
        this.ProjectConfig = projectConfig;

        if (this.ProjectConfig.FilterValues.Count != 0)
        {
            this.FilterList = this.ProjectConfig.FilterValues;
        }

        this.TempFilterList = new List<string>();
        TempFilterList.AddRange(this.FilterList);

        this.FilterAmount = TempFilterList.Count;

        this.WhenAnyValue(x => x.FilterAmount).Subscribe(_ => ChangeFilterAmount());
    }
}