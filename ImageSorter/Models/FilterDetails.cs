using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter.Models;

// For if we want the output directory name to be different from the filtername
// Other code needs to be changed to really use this though
public struct FilterDetails
{
    public string DirectoryName { get; set; } = "";
    public string FilterValue { get; set; }  = "";
    public string FullPath { get; set; }  = "";

    public FilterDetails()
    {

    }

    public FilterDetails(string Name)
    {
        this.DirectoryName = Name;
    }

    public FilterDetails(string Name,  string FilterValue)
        : this (Name)
    {
        this.FilterValue = FilterValue;
    }

    public FilterDetails(string Name, string FilterValue, string FullPath)
        : this (Name, FilterValue)
    {
        this.FullPath = FullPath;
    }
}