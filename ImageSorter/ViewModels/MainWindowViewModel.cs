using Avalonia;
using DynamicData;
using ImageSorter.Models;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class MainWindowViewModel : ReactiveObject, IScreen
{

    public RoutingState Router { get; } = new RoutingState();

    public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }

    public ReactiveCommand<Unit, IRoutableViewModel> GoBack => Router.NavigateBack;

    public static JsonSerializerOptions JsonOptions { get; private set; } = new JsonSerializerOptions {  WriteIndented = true };

    public AppState CurrentAppState { get; set; }


    public void FirstRunChecks()
    {
        var currentDirectory = Environment.CurrentDirectory;

        // Check AppState exists
        var currentAppState = new AppState();
        var appStatePath = Path.Join(currentDirectory, currentAppState.AppStateFileName);
        bool doesAppStateExist = File.Exists(appStatePath);

        if (doesAppStateExist == true)
        {
            try
            {
                currentAppState = JsonSerializer.Deserialize<AppState>(File.ReadAllText(appStatePath));

                if (currentAppState.AppStateFilePath != appStatePath)
                {
                    currentAppState.AppStateFilePath = appStatePath;
                }
            }
            // Handle bad json file
            catch (JsonException e)
            {
                currentAppState.AppStateFilePath = appStatePath;
                File.WriteAllText(appStatePath, JsonSerializer.Serialize(currentAppState, JsonOptions));
            }
        }
        else
        {
            currentAppState.AppStateFilePath = appStatePath;
            File.WriteAllText(appStatePath, JsonSerializer.Serialize(currentAppState, JsonOptions));
        }

        // Enable AppState write to file
        currentAppState.SetJsonWriterState(true);

        // Set up AppStateJsonWriter to write app state to json on changes
        // That was a useful comment I swear
        var jsonWriter = new AppStateJsonWriter();
        currentAppState.OnAppStateChanged += jsonWriter.WriteAppState;

        currentAppState.CurrentAppDirectory = currentDirectory;

        // Check ProjectConfigs exists
        var projectConfigsPath = Path.Join(currentDirectory, currentAppState.ProjectConfigDirectory);
        bool doesProjectConfigDirExists = File.Exists(projectConfigsPath);

        if (doesProjectConfigDirExists == false)
        {
            Directory.CreateDirectory(projectConfigsPath);
        }
        if (currentAppState.ProjectConfigsPath != projectConfigsPath)
        {
            currentAppState.ProjectConfigsPath = projectConfigsPath;
        }
        CurrentAppState = currentAppState;
    }

    public MainWindowViewModel()
    {
        this.FirstRunChecks();
        Router.Navigate.Execute(new ProjectSelectionViewModel(this, Router, CurrentAppState));
    }
}