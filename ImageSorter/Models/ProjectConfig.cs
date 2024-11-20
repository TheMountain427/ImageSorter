using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static ImageSorter.Models.Enums;

namespace ImageSorter.Models
{
    public class ProjectConfig : ReactiveObject
    {
        private string _projectName;
        public string ProjectName
        {
            get { return _projectName; }
            set { this.RaiseAndSetIfChanged(ref _projectName, value); }
        }

        private string _projectConfigPath;
        public string ProjectConfigPath
        {
            get { return _projectConfigPath; }
            set { this.RaiseAndSetIfChanged(ref _projectConfigPath, value); }
        }

        private List<string> _imgDirectoryPaths;
        public List<string> ImgDirectoryPaths
        {
            get { return _imgDirectoryPaths; }
            set { this.RaiseAndSetIfChanged(ref _imgDirectoryPaths, value); }
        }

        private List<string> _outputDirectoryPath;
        public List<string> OutputDirectoryPath
        {
            get { return _outputDirectoryPath; }
            set { this.RaiseAndSetIfChanged(ref _outputDirectoryPath, value); }
        }

        private ObservableCollection<string> _filterValues = new ObservableCollection<string> { "Unsorted" };
        public ObservableCollection<string> FilterValues
        {
            get { return _filterValues; }
            set { this.RaiseAndSetIfChanged(ref _filterValues, value); }
        }

        private ImageHashSet _inputImages = new ImageHashSet();
        public ImageHashSet InputImages
        {
            get { return _inputImages; }
            set { this.RaiseAndSetIfChanged(ref _inputImages, value); }
        }

        private ObservableCollection<ImageDetails> _referenceImages = new ObservableCollection<ImageDetails>();
        public ObservableCollection<ImageDetails> ReferenceImages
        {
            get { return _referenceImages; }
            set { this.RaiseAndSetIfChanged(ref _referenceImages, value); }
        }

        private ImgOrder _imageSortOrder;
        public ImgOrder ImageSortOrder
        {
            get { return _imageSortOrder; }
            set { this.RaiseAndSetIfChanged(ref _imageSortOrder, value); }
        }

        private int _currentImageIndex;
        public int CurrentImageIndex
        {
            get { return _currentImageIndex; }
            set { this.RaiseAndSetIfChanged(ref _currentImageIndex, value); }
        }

        private DateTime _lastModifiedTime = DateTime.UtcNow;
        public DateTime LastModifiedTime
        {
            get { return _lastModifiedTime; }
            set { this.RaiseAndSetIfChanged(ref _lastModifiedTime, value); }
        }


        [JsonIgnore]
        public bool JsonWriterEnabled { get; private set; } = false;

        [JsonIgnore]
        public List<ImageFileWatcher> ProjectImageWatchers { get; set; }

        [JsonIgnore]
        private JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true, IgnoreReadOnlyProperties = true };

        private EventHandler _onProjectConfigChange;
        public event EventHandler OnProjectConfigChange
        {
            add { _onProjectConfigChange += value; }
            remove { _onProjectConfigChange -= value; }
        }

        public void SetLastModifiedTime()
        {
            LastModifiedTime = DateTime.UtcNow;
        }

        public void SetLastModifiedOnAppClose(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            LastModifiedTime = DateTime.UtcNow;
        }


        public void SetJsonWriterState(bool value)
        {
            this.JsonWriterEnabled = value;
        }


        // Need to test image watcher, this is probably unneeded
        public void InputImageChangeEvent(object sender, EventArgs e)
        {
            // Forwarding ImageWatcher events to ImageHashSet
            // Kinda hacky way to force an onChanged event and have JSON writer fire
            if (e is RenamedEventArgs renamedEventArgs)
            {
                this.InputImages.OnImageRenamed(sender, renamedEventArgs);
                this.SetLastModifiedTime();
            }
            else if (e is FileSystemEventArgs fileSystemEventArgs)
            {
                switch (fileSystemEventArgs.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        this.InputImages.OnImageCreated(sender, fileSystemEventArgs);
                        this.SetLastModifiedTime();
                        break;
                    case WatcherChangeTypes.Deleted:
                        this.InputImages.OnImageDeleted(sender, fileSystemEventArgs);
                        this.SetLastModifiedTime();
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetImageFilterValue(ImageDetails imageDetails, string filterValue)
        {
            var imageToSet = this.InputImages.First(x => x == imageDetails);
            imageToSet.FilteredValue = filterValue;
            this.SetLastModifiedTime();
        }

        public void WriteProjectConfigState(ProjectConfig ProjectConfig)
        {
            if (ProjectConfig.JsonWriterEnabled)
            {
                File.WriteAllText(ProjectConfig.ProjectConfigPath, JsonSerializer.Serialize(ProjectConfig, this.JsonOptions));
            }
        }


        public ProjectConfig()
        {
            this.Changed.Subscribe(_ => WriteProjectConfigState(this));
        }
    }
}
