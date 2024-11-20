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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static ImageSorter.Models.Enums;

namespace ImageSorter.Models
{
    public class ProjectConfig : ReactiveObject
    {
        private string _projectName;
        private DateTime _lastModifiedTime = DateTime.UtcNow;
        private string _projectConfigPath;
        private List<string> _imgDirectoryPaths;
        private List<string> _outputDirectoryPath;
        private ObservableCollection<string> _filterValues = new ObservableCollection<string> { "Unsorted" };
        private ImageHashSet _inputImages = new ImageHashSet();
        private ObservableCollection<ImageDetails> _referenceImages = new ObservableCollection<ImageDetails>();
        private ImgOrder _imageSortOrder;
        private int _currentImageIndex;

        [JsonIgnore]
        public bool JsonWriterEnabled { get; private set; } = false;
        [JsonIgnore]
        public List<ImageFileWatcher> ProjectImageWatchers { get; set; }
        [JsonInclude]
        public string ProjectName
        {
            get => _projectName;
            set
            {
                _projectName = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
                this.SetLastModifiedTime();
            }
        }
        [JsonInclude]
        public string ProjectConfigPath
        {
            get => _projectConfigPath;
            set
            {
                _projectConfigPath = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
                this.SetLastModifiedTime();
            }
        }
        [JsonInclude]
        public List<string> ImgDirectoryPaths
        {
            get => _imgDirectoryPaths;
            set
            {
                _imgDirectoryPaths = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
                this.SetLastModifiedTime();
            }
        }
        [JsonInclude]
        public List<string> OutputDirectoryPath
        {
            get => _outputDirectoryPath;
            set
            {
                _outputDirectoryPath = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
                this.SetLastModifiedTime();
            }
        }
        [JsonInclude]
        public ObservableCollection<string> FilterValues
        {
            get => _filterValues;
            set
            {
                this.RaiseAndSetIfChanged(ref _filterValues, value);
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
                this.SetLastModifiedTime();
            }
        }
        [JsonInclude]
        public ImageHashSet InputImages
        {
            get => _inputImages;
            set
            {
                _inputImages = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
                this.SetLastModifiedTime();
            }
        }
        [JsonInclude]
        public ObservableCollection<ImageDetails> ReferenceImages
        {
            get => _referenceImages;
            set
            {
                _referenceImages = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
                this.SetLastModifiedTime();
            }
        }
        [JsonInclude]
        public ImgOrder ImageSortOrder
        {
            get => _imageSortOrder;
            set
            {
                _imageSortOrder = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
                this.SetLastModifiedTime();
            }
        }
        [JsonInclude]
        public int CurrentImageIndex
        {
            get => _currentImageIndex;
            set
            {
                _currentImageIndex = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
                this.SetLastModifiedTime();
            }
        }
        [JsonInclude]
        public DateTime LastModifiedTime
        {
            get => _lastModifiedTime;
            protected set
            {
                _lastModifiedTime = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
            }
        }

        private EventHandler _onProjectConfigChange;
        public event EventHandler OnProjectConfigChange
        {
            add { _onProjectConfigChange += value; }
            remove {  _onProjectConfigChange -= value; }
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

        
    }
}
