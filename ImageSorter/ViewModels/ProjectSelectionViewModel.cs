using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class ProjectSelectionViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "ProjectSelection";

    public TopLevel topLevel = App.TopLevel;

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
                    doFoldersExist = Directory.Exists(folder);

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
                    doFoldersExist = Directory.Exists(folder);

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

    public ICommand GoToWorkspace { get; }
    private void _goToWorkspace()
    {
        MainRouter.Navigate.Execute(new WorkspaceViewModel(this.HostScreen, this.MainRouter));
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
            foreach (var folder in DirectoryPaths)
            {
                text += $"{folder}| ";
            }
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

    // **** Debug **** //
    public void dbgSetProjectSettings()
    {
        ProjectSettingsSet = true;
    }
    public void dbgGoToWorkspace()
    {
        MainRouter.Navigate.Execute(new WorkspaceViewModel(this.HostScreen, this.MainRouter));
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


    public ProjectSelectionViewModel(IScreen screen, RoutingState router)
    {
        MainRouter = router;
        HostScreen = screen;
        //var topLevel = TopLevel.GetTopLevel(this);

        var AreProjectSettingsSet = this.WhenAnyValue(x => x.ProjectSettingsSet);
        GoToWorkspace = ReactiveCommand.Create(_goToWorkspace, AreProjectSettingsSet);

        // Watch the output of folder browser and set UI text if it changes
        this.WhenAnyValue(x => x.ImgDirectories).Subscribe(_ => SetTextPath(ImgDirectories, "ImgPath"));
        this.WhenAnyValue(x => x.OutDirectories).Subscribe(_ => SetTextPath(OutDirectories, "OutPath"));

        this.WhenAnyValue(x => x.ImgPathText, x => x.OutPathText, x => x.ProjectNameText).Subscribe(_ => UpdateCanGoToWorkspace());
        // Listen to changes of MailAddress and Password and update CanNavigateNext accordingly
        //this.WhenAnyValue(x => x.MailAddress, x => x.Password)
        //    .Subscribe(_ => UpdateCanNavigateNext());



    }

}