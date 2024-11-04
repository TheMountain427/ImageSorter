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

public class ProjectSelectionViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "ProjectSelection";

    public TopLevel topLevel = App.TopLevel;

    public ProjectConfig ProjectConfig { get; protected set; }

    private bool didLoadRecentProject { get; set; }

    private bool _projectNameSet = false;
    public bool ProjectNameSet
    {
        get { return _projectNameSet; }
        protected set { this.RaiseAndSetIfChanged(ref _projectNameSet, value); }
    }

    private bool _imgPathSelected = false;
    public bool ImgPathSelected
    {
        get { return _imgPathSelected; }
        protected set { this.RaiseAndSetIfChanged(ref _imgPathSelected, value); }
    }

    private bool _outPathSelected = false;
    public bool OutPathSelected
    {
        get { return _outPathSelected; }
        protected set { this.RaiseAndSetIfChanged(ref _outPathSelected, value); }
    }

    private bool _projectSettingsSet = false;
    public bool ProjectSettingsSet
    {
        get { return _projectSettingsSet; }
        protected set { this.RaiseAndSetIfChanged(ref _projectSettingsSet, value); }
    }

    private string _projectNameErrorText = "";
    public string ProjectNameErrorText
    {
        get { return _projectNameErrorText; }
        protected set { this.RaiseAndSetIfChanged(ref _projectNameErrorText, value); }
    }

    private string _imgErrorText = "";
    public string ImgErrorText
    {
        get { return _imgErrorText; }
        protected set { this.RaiseAndSetIfChanged(ref _imgErrorText, value); }
    }

    private string _outErrorText = "";
    public string OutErrorText
    {
        get { return _outErrorText; }
        protected set { this.RaiseAndSetIfChanged(ref _outErrorText, value); }
    }

    private bool _imgPathTextBoxLostFocus = false;
    public bool ImgPathTextBoxLostFocus
    {
        get { return _imgPathTextBoxLostFocus; }
        set { this.RaiseAndSetIfChanged(ref _imgPathTextBoxLostFocus, value); }
    }

    private bool _outPathTextBoxLostFocus = false;
    public bool OutPathTextBoxLostFocus
    {
        get { return _outPathTextBoxLostFocus; }
        set { this.RaiseAndSetIfChanged(ref _outPathTextBoxLostFocus, value); }
    }

    [Required]
    private string _projectNameText;
    public string ProjectNameText
    {
        get { return _projectNameText; }
        set { this.RaiseAndSetIfChanged(ref _projectNameText, value); }
    }

    private List<string> _imgDirectories;
    public List<string> ImgDirectories
    {
        get { return _imgDirectories; }
        protected set { this.RaiseAndSetIfChanged(ref _imgDirectories, value); }
    }

    [Required]
    private string _imgPathText;
    public string ImgPathText
    {
        get { return _imgPathText; }
        set { this.RaiseAndSetIfChanged(ref _imgPathText, value); }
    }

    private List<string> _outDirectories;
    public List<string> OutDirectories
    {
        get { return _outDirectories; }
        protected set { this.RaiseAndSetIfChanged(ref _outDirectories, value); }
    }

    [Required]
    private string _outPathText;
    public string OutPathText
    {
        get { return _outPathText; }
        set { this.RaiseAndSetIfChanged(ref _outPathText, value); }
    }

    public ICommand GoToWorkspace { get; }
    private void _goToWorkspace()
    {
        InitializeProject();
        MainRouter.Navigate.Execute(new WorkspaceViewModel(this.HostScreen, this.MainRouter, this.CurrentAppState, ProjectConfig));
    }

    private List<string> _recentProjects = new List<string> { "Recent Projects" };
    public List<string> RecentProjects
    {
        get { return _recentProjects; }
        protected set { this.RaiseAndSetIfChanged(ref _recentProjects, value); }
    }
    // workaround since Combobox is not displaying a default selected item properly
    // actually I need to use this
    private string _selectedRecentProject = "Recent Projects";
    public string SelectedRecentProject
    {
        get { return _selectedRecentProject; }
        set { this.RaiseAndSetIfChanged(ref _selectedRecentProject, value); }
    }

    public List<ImageDetails> NewImagesFound { get; set; } = new List<ImageDetails>();
    public List<ImageDetails> OldImagesRemoved { get; set; } = new List<ImageDetails>();
    // Old detail > New detail
    public List<KeyValuePair<ImageDetails, ImageDetails>> UpdatedImages { get; set; } = new List<KeyValuePair<ImageDetails, ImageDetails>>();


    public void UpdateCanGoToWorkspace()
    {
        if (string.IsNullOrEmpty(ProjectNameText) == false)
        {
            bool validProjectName = false;
            // invalid file name characters. idk why escaping needed to be so jank
            if (Regex.IsMatch(ProjectNameText, "[\\\\/:*?\"<>|]"))
            {
                ProjectNameErrorText = "Invalid Project Name";
            }
            else
            {
                validProjectName = true;
            }
            ProjectNameSet = validProjectName;
        }
        // Checks for manually typed in paths
        if (string.IsNullOrEmpty(ImgPathText) == false)
        {
            bool doFoldersExist = false;
            if (ImgPathText.Contains('|'))
            {
                var multipleManualFolders = ImgPathText.Split('|');

                foreach (var folder in multipleManualFolders)
                {
                    doFoldersExist = Directory.Exists(folder.Trim());

                    if (!doFoldersExist)
                    {
                        ImgErrorText = "Invalid Image Path";
                        break; // fail fast if not exists
                    }
                }
            }
            else
            {
                doFoldersExist = Directory.Exists(ImgPathText);
                if (!doFoldersExist)
                {
                    ImgErrorText = "Invalid Image Path";
                }
            }
            ImgPathSelected = doFoldersExist;
        }

        // Checks for manually typed in paths
        if (string.IsNullOrEmpty(OutPathText) == false)
        {
            bool doFoldersExist = false;
            if (OutPathText.Contains('|'))
            {
                var multipleManualFolders = OutPathText.Split('|');

                foreach (var folder in multipleManualFolders)
                {
                    doFoldersExist = Directory.Exists(folder.Trim());

                    if (!doFoldersExist)
                    {
                        OutErrorText = "Invalid Output Path";
                        break; // fail fast if not exists
                    }
                }
            }
            else
            {
                doFoldersExist = Directory.Exists(OutPathText);
                if (!doFoldersExist)
                {
                    OutErrorText = "Invalid Output Path";
                }
            }
            OutPathSelected = doFoldersExist;
        }
        ProjectNameErrorText = ProjectNameSet ? "" : ProjectNameErrorText;
        ImgErrorText = ImgPathSelected ? "" : ImgErrorText;
        OutErrorText = OutPathSelected ? "" : OutErrorText;

        ProjectSettingsSet = ProjectNameSet && ImgPathSelected && OutPathSelected;
    }



    private void InitializeProject()
    {
        if (didLoadRecentProject)
        {
            CurrentAppState.CurrentProjectName = ProjectNameText;
            var configPath = TryGetProjectConfigPath(CurrentAppState, ProjectNameText);
            if (configPath is null)
            {
                throw new Exception("Exploded. Some how the config was not found. Shits fucked.");
            }
            else
            {
                CurrentAppState.CurrentProjectConfigPath = configPath;
            }
        }
        else
        {
            ProjectNameText = CheckProjectNameDuplicates(CurrentAppState, ProjectNameText).Trim();

            // hopefully fix for when: load recent > change name > now no projectconfig > no directories were set > die
            try
            {
                ImgDirectories = ImgDirectories is null || ImgDirectories.Count == 0 ? ImgPathText.Split('|').ToList() : ImgDirectories;
                OutDirectories = OutDirectories is null || OutDirectories.Count == 0 ? OutPathText.Split('|').ToList() : OutDirectories;
            }
            catch (Exception e)
            {
                throw new Exception("Literally no paths exists anywhere, but all checks failed and somehow we are still trying to start a project", e);
            }


            var projConfig = new ProjectConfig
            {
                ProjectName = ProjectNameText,
                ImgDirectoryPaths = ImgDirectories,
                OutputDirectoryPath = OutDirectories
            };

            var pathToConfig = Path.Join(CurrentAppState.ProjectConfigsPath, ProjectNameToFileName(ProjectNameText));
            File.WriteAllText(pathToConfig, JsonSerializer.Serialize(projConfig, new JsonSerializerOptions { WriteIndented = true }));
            CurrentAppState.CurrentProjectConfigPath = pathToConfig;
        }

        var projectConfig = GetProjectConfigFromJson(CurrentAppState.CurrentProjectConfigPath);

        // Load images details from image input directories, only unique file names
        if (!didLoadRecentProject) // oops, was duplicating images each load
        {
            foreach (var inputPath in projectConfig.ImgDirectoryPaths)
            {
                // fuck them other image types
                var images = Directory.EnumerateFiles(inputPath).Where(x => x.EndsWith(".jpeg") || x.EndsWith(".png") || x.EndsWith(".jpg"));
                if (images.Count() > 0)
                {
                    foreach (var imagePath in images)
                    {
                        projectConfig.InputImages.Add(new ImageDetails(imagePath));
                    }
                }
            }

        }
        else
        {
            // Loading a recent project, try to fix missing values, remove old images, and load new ones
            // Placing any issues into NewImagesFound, OldImagesMissing, UpdatedImages properties for use later
            var foundImagePaths = new List<string>();

            foreach (var inputPath in projectConfig.ImgDirectoryPaths)
            {
                foundImagePaths.AddRange(Directory.EnumerateFiles(inputPath).Where(x => x.EndsWith(".jpeg") || x.EndsWith(".png") || x.EndsWith(".jpg")));
            }

            // Try to fix any images in the config with missing paths
            var missingPaths = new List<ImageDetails>();
            missingPaths.AddRange(projectConfig.InputImages.Where(x => x.FilePath is null && x.FileName is not null));

            if (missingPaths.Any())
            {
                foreach (var missing in missingPaths)
                {
                    var foundMissingPath = foundImagePaths.FirstOrDefault(x => Path.GetFileName(x) == missing.FileName);
                    var oldImage = projectConfig.InputImages.First(x => x.FileName == missing.FileName);

                    if (!string.IsNullOrEmpty(foundMissingPath))
                    {
                        // We found the image and will fix the path
                        var updatedImage = new ImageDetails(foundMissingPath);
                        // Add a Old>New keypair to use as an alert later
                        UpdatedImages.Add(new KeyValuePair<ImageDetails, ImageDetails>(oldImage, updatedImage));
                        projectConfig.InputImages.Remove(oldImage);
                        projectConfig.InputImages.Add(updatedImage);
                    }
                    else
                    {
                        // Image was not found, add an alert and remove it from the config
                        OldImagesRemoved.Add(oldImage);
                        projectConfig.InputImages.Remove(oldImage);
                    }
                }
            }

            // Try to fix any images with missing names
            var missingNames = new List<ImageDetails>();
            missingNames.AddRange(projectConfig.InputImages.Where(x => x.FileName is null && x.FilePath is not null));

            if (missingNames.Any())
            {
                foreach (var missing in missingNames)
                {
                    var oldImage = projectConfig.InputImages.First(x => x.FilePath == missing.FilePath);
                    var fileName = Path.GetFileName(missing.FilePath);

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        // We found the image name
                        var updatedImage = new ImageDetails(fileName);
                        UpdatedImages.Add(new KeyValuePair<ImageDetails, ImageDetails>(oldImage, updatedImage));
                        projectConfig.InputImages.Remove(oldImage);
                        projectConfig.InputImages.Add(updatedImage);
                    }
                    else
                    {
                        // File doesn't have a name??? shouldn't happen but we will log it and remove it
                        OldImagesRemoved.Add(oldImage);
                        projectConfig.InputImages.Remove(oldImage);
                    }
                }
            }

            // Load any new Images not currently in the project config
            // Returns filepaths in foundImagePaths that are not present in InputImages(x => x.FilePath)
            var newImages = foundImagePaths.Except(projectConfig.InputImages.Select(x => x.FilePath));

            if (newImages.Any())
            {
                foreach (var newImagePath in newImages)
                {
                    // We found new images, log em and add em
                    var newImageDetail = new ImageDetails(newImagePath);
                    NewImagesFound.Add(newImageDetail);
                    projectConfig.InputImages.Add(newImageDetail);
                }
            }

            // Remove any images from project config that no longer exist in the directories
            var removedImages = projectConfig.InputImages.Select(x => x.FilePath).Except(foundImagePaths);

            if (removedImages.Any())
            {
                foreach (var missingImage in removedImages)
                {
                    // We found an image that no longer exists and could not be fixed, bag em and tag em
                    var oldImage = ProjectConfig.InputImages.First(x => x.FilePath == missingImage);
                    OldImagesRemoved.Add(oldImage);
                    projectConfig.InputImages.Remove(oldImage);
                }
            }
        }

        // Set up jsonWriter to write projectconfig to a json when it changes
        // hopefully carries over into the workspace
        var jsonWriter = new ProjectConfigJsonWriter();
        projectConfig.OnProjectConfigChange += jsonWriter.WriteProjectConfigState;


        // Enable Json Writer for project config
        // should also trigger a Json write
        // nvm I have that set not to raise an event
        projectConfig.SetJsonWriterState(true);

        // This will trigger it
        projectConfig.ProjectConfigPath = CurrentAppState.CurrentProjectConfigPath;

        this.ProjectConfig = projectConfig;

        // Change last modified time on App close, triggers a JSON write
        App.Desktop.Exit += ProjectConfig.SetLastModifiedOnAppClose;

        // Initilize file watchers to watch for changes in the input images
        this.ProjectConfig.ProjectImageWatchers = new List<ImageFileWatcher>();
        foreach (var path in this.ProjectConfig.ImgDirectoryPaths)
        {
            var watcher = new ImageFileWatcher(path);
            watcher.ImageWatcher.Renamed += this.ProjectConfig.InputImageChangeEvent;
            watcher.ImageWatcher.Created += this.ProjectConfig.InputImages.OnImageCreated;
            watcher.ImageWatcher.Deleted += this.ProjectConfig.InputImages.OnImageDeleted;
            ProjectConfig.ProjectImageWatchers.Add(watcher);
        }
    }



    // Set the text that will be shown in the UI path text boxes
    private void SetTextPath(List<string> DirectoryPaths, string destination)
    {
        if (DirectoryPaths is null)
            return;

        string text = "";

        if (DirectoryPaths.Count == 1)
        {
            text = DirectoryPaths[0];
        }
        else
        {
            text = SetDirectoryPathText(DirectoryPaths);
        }

        switch (destination)
        {
            case "ImgPath":
                ImgPathText = text;
                break;
            case "OutPath":
                OutPathText = text;
                break;
            default:
                break;
        }
    }



    private static string SetDirectoryPathText(List<string> DirectoryPaths)
    {
        var text = "";
        foreach (var folder in DirectoryPaths)
        {
            text += $"{folder}|";
        }
        text = text.TrimEnd('|');

        return text;
    }



    public async void BrowseFiles(string msg)
    {
        // Set settings for folder browser, default location is Desktop
        var options = new FolderPickerOpenOptions
        {
            AllowMultiple = true,
            SuggestedStartLocation = await App.TopLevel.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Desktop)
        };

        // If ImgPath had been set, open there instead. If not try OutPath and open there.
        if (ImgDirectories is not null && ImgDirectories.Count != 0)
        {
            var tryGetFolder = await App.TopLevel.StorageProvider.TryGetFolderFromPathAsync(ImgDirectories[0]);
            options.SuggestedStartLocation = tryGetFolder is not null ? tryGetFolder : options.SuggestedStartLocation;
        }
        else if (OutDirectories is not null && OutDirectories.Count != 0)
        {
            var tryGetFolder = await App.TopLevel.StorageProvider.TryGetFolderFromPathAsync(OutDirectories[0]);
            options.SuggestedStartLocation = tryGetFolder is not null ? tryGetFolder : options.SuggestedStartLocation;
        }

        var SelectedDirectories = await App.TopLevel.StorageProvider.OpenFolderPickerAsync(options);

        var SelectedDirectoriesPaths = new List<string>();

        if (SelectedDirectories is not null)
        {
            SelectedDirectoriesPaths.AddRange(SelectedDirectories.Select(x => x.Path.LocalPath));
        }

        // Which textbox should the file paths go to?
        switch (msg)
        {
            case "ImgPath":
                ImgDirectories = SelectedDirectoriesPaths;
                break;
            case "OutPath":
                OutDirectories = SelectedDirectoriesPaths;
                break;
            default:
                break;
        }
    }



    private async void GetRecentProjects(AppState appState)
    {
        if (appState is not null && appState.ProjectConfigsPath is not null)
        {
            var projectConfigs = Directory.GetFiles(appState.ProjectConfigsPath, "*.json");

            if (projectConfigs.Length != 0)
            {
                var foundRecentProjects = new List<KeyValuePair<string, DateTimeOffset>>();

                foreach (var jsonPath in projectConfigs)
                {
                    try
                    {
                        var projJSON = JsonDocument.Parse(File.ReadAllText(jsonPath));
                        var projName = projJSON.RootElement.GetProperty("ProjectName");

                        if (!string.IsNullOrEmpty(projName.ToString()))
                        {
                            bool doesTimeExist = DateTime.TryParse(projJSON.RootElement.GetProperty("LastModifiedTime").ToString(), out var mod);

                            if (doesTimeExist)
                            {
                                foundRecentProjects.Add(new KeyValuePair<string, DateTimeOffset>(projName.ToString(), mod));
                            }
                        }
                    }
                    // Handle Invalid Json
                    catch (JsonException e)
                    {
                        continue;
                    }
                    // Handle JSON does not contain property
                    catch (KeyNotFoundException e)
                    {
                        continue;
                    }
                }

                if (foundRecentProjects.Count != 0)
                {
                    // Add 5 most recent projects to RecentProjects
                    var recentProjects = foundRecentProjects.OrderByDescending(x => x.Value).Select(x => x.Key).Take(5).ToList();
                    RecentProjects.AddRange(recentProjects);
                    CurrentAppState.RecentProjectNames = recentProjects;
                }
            }
        }
    }



    private void TryLoadRecentProject()
    {
        if (SelectedRecentProject != "Recent Projects")
        {
            var projectConfigPath = CurrentAppState.ProjectConfigsPath;
            var jsonConfigPath = Path.Join(projectConfigPath, ProjectNameToFileName(SelectedRecentProject));
            var doesProjExist = File.Exists(jsonConfigPath);

            if (doesProjExist)
            {
                var loadedProj = new ProjectConfig();
                try
                {
                    loadedProj = GetProjectConfigFromJson(jsonConfigPath);
                }
                // handle bad json 
                catch (JsonException e)
                {
                    return;
                }

                if (loadedProj is not null && TestProjectConfig(loadedProj))
                {
                    ProjectNameText = loadedProj.ProjectName;
                    ImgPathText = SetDirectoryPathText(loadedProj.ImgDirectoryPaths);
                    OutPathText = SetDirectoryPathText(loadedProj.OutputDirectoryPath);
                    this.didLoadRecentProject = true;
                }
            }
        }
    }



    private void CheckLoadedProject()
    {
        if (didLoadRecentProject == true)
        {
            didLoadRecentProject = false;
        }
    }



    // **** Debug **** //
    public void dbgSetProjectSettings()
    {
        ProjectSettingsSet = true;
    }
    public void dbgGoToWorkspace()
    {
        MainRouter.Navigate.Execute(new WorkspaceViewModel(this.HostScreen, this.MainRouter, this.CurrentAppState, this.ProjectConfig));
    }



    // **** ToolTips **** //
    public string ProjectNameHelp { get; } = "Enter the name for a the project.";
    public string ProjectNameBoxHelp { get; } = "The name of the project.";
    public string ImgPathHelp { get; } = "Use the Browse button or enter the full path to the directory containing images to be sorted.";
    public string ImgPathBoxHelp { get; } = "Path to image directory.";
    public string OutPathHelp { get; } = "Use the Browse button or enter the full path to the directory that images should be sorted into.";
    public string OutPathBoxHelp { get; } = "Path to output directory.";
    public string BrowseButtonHelp { get; } = "Browse the local filesystem.";
    public string LoadConfigHelp { get; } = "Select a ProjectConfig file associated with an existing project. This will load that project.";
    public string RecentProjectsHelp { get; } = "A list of recent projects.";
    public string BeginHelp { get; } = "Go to the workspace using the current settings.";



    public ProjectSelectionViewModel(IScreen screen, RoutingState router, AppState appState)
    {
        MainRouter = router;
        HostScreen = screen;
        CurrentAppState = appState;

        GetRecentProjects(appState);
        // Try to load selected recent project config
        this.WhenAnyValue(x => x.SelectedRecentProject).Subscribe(_ => TryLoadRecentProject());


        var AreProjectSettingsSet = this.WhenAnyValue(x => x.ProjectSettingsSet);
        GoToWorkspace = ReactiveCommand.Create(_goToWorkspace, AreProjectSettingsSet);

        // Watch the output of folder browser and set UI text if it changes
        this.WhenAnyValue(x => x.ImgDirectories).Subscribe(_ => SetTextPath(ImgDirectories, "ImgPath"));
        this.WhenAnyValue(x => x.OutDirectories).Subscribe(_ => SetTextPath(OutDirectories, "OutPath"));

        this.WhenAnyValue(x => x.ImgPathText, x => x.OutPathText, x => x.ProjectNameText).Subscribe(_ => UpdateCanGoToWorkspace());

        // Should handle when recent project is loaded but values are changed => don't use loaded project config
        this.WhenAnyValue(x => x.ImgPathText, x => x.OutPathText, x => x.ProjectNameText).Subscribe(_ => CheckLoadedProject());



    }

}