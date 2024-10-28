using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ImageSorter.Models
{
    public class ProjectConfig
    {
        private string _projectName;
        private string _projectConfigPath;
        private List<string> _imgDirectoryPaths;
        private List<string> _outputDirectoryPath;
        private List<string> _filterValues = new List<string> { "Unsorted" };
        private ImageHashSet _inputImages = new ImageHashSet();
        private ImageHashSet _referenceImages = new ImageHashSet();
        [JsonIgnore]
        public bool JsonWriterEnabled { get; private set; } = false;

        public string ProjectName
        {
            get => _projectName;
            set
            {
                _projectName = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
            }
        }

        public string ProjectConfigPath
        {
            get => _projectConfigPath;
            set
            {
                _projectConfigPath = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
            }
        }

        public List<string> ImgDirectoryPaths
        {
            get => _imgDirectoryPaths;
            set
            {
                _imgDirectoryPaths = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
            }
        }

        public List<string> OutputDirectoryPath
        {
            get => _outputDirectoryPath;
            set
            {
                _outputDirectoryPath = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
            }
        }

        public List<string> FilterValues
        {
            get => _filterValues;
            set
            {
                _filterValues = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
            }
        }

        public ImageHashSet InputImages
        {
            get => _inputImages;
            set
            {
                _inputImages = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
            }
        }

        public ImageHashSet ReferenceImages
        {
            get => _referenceImages;
            set
            {
                _referenceImages = value;
                _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
            }
        }

        private EventHandler _onProjectConfigChange;
        public event EventHandler OnProjectConfigChange
        {
            add
            {
                _onProjectConfigChange += value;
            }
            remove
            {
                _onProjectConfigChange -= value;
            }
        }

        public void SetJsonWriterState(bool value)
        {
            this.JsonWriterEnabled = value;
        }
    }
}
