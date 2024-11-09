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
        private string _currentAppDirectory;
        private string _appStateFileName = "AppState.json";
        private string _appStateFilePath;
        private string _projectConfigDirectory = "ProjectConfigs";
        private string _projectConfigsPath;
        private List<string> _recentProjectNames;
        private string _currentProjectName;
        private string _currentProjectConfigPath;
        private bool _filterSidePanelOpen;

        public string LastReferenceImagePath { get; set; }
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

        public void SetJsonWriterState(bool value)
        {
            this.JsonWriterEnabled = value;
        }

    }
}
