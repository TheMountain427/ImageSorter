using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ImageSorter.Models
{
    public class AppState : ReactiveObject
    {
        public bool DebugMode { get; } = false;


        private string _currentAppDirectory;
        private string _appStateFileName = "AppState.json";
        private string _appStateFilePath;
        private string _projectConfigDirectory = "ProjectConfigs";
        private string _projectConfigsPath;
        private List<string> _recentProjectNames;
        private string _currentProjectName;
        private string _currentProjectConfigPath;
        private bool _filterSidePanelOpen;
        private double _windowWidth;
        private double _windowHeight;

        public string LastReferenceImagePath { get; set; }


        [JsonIgnore]
        private bool _isWorkSpaceOverlayEnabled = false;
        [JsonIgnore]
        public bool IsWorkSpaceOverlayEnabled
        {
            get { return _isWorkSpaceOverlayEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isWorkSpaceOverlayEnabled, value); }
        }

        [JsonIgnore]
        public bool JsonWriterEnabled { get; private set; } = false;


        public string CurrentAppDirectory
        {
            get => _currentAppDirectory;
            set
            {
                _currentAppDirectory = value;
                _onAppStateChanged?.Invoke(this, EventArgs.Empty); // Instead of new EventArgs() for performance
            }
        }

        public string AppStateFileName
        {
            get => _appStateFileName;
            set
            {
                _appStateFileName = value;
                _onAppStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string AppStateFilePath
        {
            get => _appStateFilePath;
            set
            {
                _appStateFilePath = value;
                _onAppStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string ProjectConfigDirectory
        {
            get => _projectConfigDirectory;
            set
            {
                _projectConfigDirectory = value;
                _onAppStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string ProjectConfigsPath
        {
            get => _projectConfigsPath;
            set
            {
                _projectConfigsPath = value;
                _onAppStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        public List<string> RecentProjectNames
        {
            get => _recentProjectNames;
            set
            {
                _recentProjectNames = value;
                _onAppStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        public string CurrentProjectName
        {
            get => _currentProjectName;
            set
            {
                _currentProjectName = value;
                _onAppStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string CurrentProjectConfigPath
        {
            get => _currentProjectConfigPath;
            set
            {
                _currentProjectConfigPath = value;
                _onAppStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool FilterSidePanelOpen
        {
            get => _filterSidePanelOpen;
            set
            {
                this.RaiseAndSetIfChanged(ref _filterSidePanelOpen, value);
                _onAppStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private EventHandler _onAppStateChanged;
        public event EventHandler OnAppStateChanged
        {
            add
            {
                _onAppStateChanged += value;
            }
            remove
            {
                _onAppStateChanged -= value;
            }
        }

        public double WindowWidth
        {
            get { return _windowWidth; }
            set
            {
                this.RaiseAndSetIfChanged(ref _windowWidth, value);
                _onAppStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public double WindowHeight
        {
            get { return _windowHeight; }
            set
            {
                this.RaiseAndSetIfChanged(ref _windowHeight, value);
                _onAppStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetJsonWriterState(bool value)
        {
            this.JsonWriterEnabled = value;
        }

    }
}
