using DynamicData.Experimental;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter.Models
{
    public class ImageFileWatcher
    {

        public FileSystemWatcher ImageWatcher { get; protected set; }

        public ImageFileWatcher(string path)
        {
            ImageWatcher = new FileSystemWatcher()
            {
                Path = path,
                //NotifyFilter = NotifyFilters.FileName,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };

            //ImageWatcher.Changed += this.OnChanged;
            //ImageWatcher.Created += this.OnCreated;
            //ImageWatcher.Deleted += this.OnDeleted;
            //ImageWatcher.Renamed += this.OnRenamed;
            //ImageWatcher.Error += this.OnError;
        }
    }
}
