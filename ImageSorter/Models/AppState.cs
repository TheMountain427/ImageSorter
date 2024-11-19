using Avalonia.Layout;
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
        public bool DebugMode { get; } = true;

        private string _appStateFileName = "AppState.json";
        public string AppStateFileName
        {
            get { return _appStateFileName; }
            set { this.RaiseAndSetIfChanged(ref _appStateFileName, value); }
        }

        private string _projectConfigDirectory = "ProjectConfigs";
        public string ProjectConfigDirectory
        {
            get { return _projectConfigDirectory; }
            set { this.RaiseAndSetIfChanged(ref _projectConfigDirectory, value); }
        }

        private string _currentAppDirectory = "";
        public string CurrentAppDirectory
        {
            get { return _currentAppDirectory; }
            set { this.RaiseAndSetIfChanged(ref _currentAppDirectory, value); }
        }

        private string _appStateFilePath = "";
        public string AppStateFilePath
        {
            get { return _appStateFilePath; }
            set { this.RaiseAndSetIfChanged(ref _appStateFilePath, value); }
        }

        private string _projectConfigsPath = "";
        public string ProjectConfigsPath
        {
            get { return _projectConfigsPath; }
            set { this.RaiseAndSetIfChanged(ref _projectConfigsPath, value); }
        }

        private string _currentProjectName = "";
        public string CurrentProjectName
        {
            get { return _currentProjectName; }
            set { this.RaiseAndSetIfChanged(ref _currentProjectName, value); }
        }

        private string _currentProjectConfigPath = "";
        public string CurrentProjectConfigPath
        {
            get { return _currentProjectConfigPath; }
            set { this.RaiseAndSetIfChanged(ref _currentProjectConfigPath, value); }
        }

        private string _lastReferenceImagePath = "";
        public string LastReferenceImagePath
        {
            get { return _lastReferenceImagePath; }
            set { this.RaiseAndSetIfChanged(ref _lastReferenceImagePath, value); }
        }

        private List<string> _recentProjectNames = new List<string>();
        public List<string> RecentProjectNames
        {
            get { return _recentProjectNames; }
            set { this.RaiseAndSetIfChanged(ref _recentProjectNames, value); }
        }

        [JsonIgnore]
        private bool _isWorkSpaceOverlayEnabled = false;
        [JsonIgnore]
        public bool IsWorkSpaceOverlayEnabled
        {
            get { return _isWorkSpaceOverlayEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isWorkSpaceOverlayEnabled, value); }
        }


        private bool _filterSidePanelOpen;
        public bool FilterSidePanelOpen
        {
            get { return _filterSidePanelOpen; }
            set { this.RaiseAndSetIfChanged(ref _filterSidePanelOpen, value); }
        }

        private double _windowWidth;
        public double WindowWidth
        {
            get { return _windowWidth; }
            set { this.RaiseAndSetIfChanged(ref _windowWidth, value); }
        }

        private double _windowHeight;
        public double WindowHeight
        {
            get { return _windowHeight; }
            set { this.RaiseAndSetIfChanged(ref _windowHeight, value); }
        }

        private HorizontalAlignment _thumbnailHorizontalAlign = HorizontalAlignment.Center;
        public HorizontalAlignment ThumbnailHorizontalAlign
        {
            get { return _thumbnailHorizontalAlign; }
            set { this.RaiseAndSetIfChanged(ref _thumbnailHorizontalAlign, value); }
        }

        private VerticalAlignment _thumbnailVerticalAlign = VerticalAlignment.Top;
        public VerticalAlignment ThumbnailVerticalAlign
        {
            get { return _thumbnailVerticalAlign; }
            set { this.RaiseAndSetIfChanged(ref _thumbnailVerticalAlign, value); }
        }

        private HorizontalAlignment _controlsHorizontalAlign = HorizontalAlignment.Center;
        public HorizontalAlignment ControlsHorizontalAlign
        {
            get { return _controlsHorizontalAlign; }
            set { this.RaiseAndSetIfChanged(ref _controlsHorizontalAlign, value); }
        }

        private VerticalAlignment _controlsVerticalAlign = VerticalAlignment.Bottom;
        public VerticalAlignment ControlsVerticalAlign
        {
            get { return _controlsVerticalAlign; }
            set { this.RaiseAndSetIfChanged(ref _controlsVerticalAlign, value); }
        }

        [JsonIgnore]
        public bool JsonWriterEnabled { get; private set; } = false;

        public void SetJsonWriterState(bool value)
        {
            this.JsonWriterEnabled = value;
        }
    }
}
