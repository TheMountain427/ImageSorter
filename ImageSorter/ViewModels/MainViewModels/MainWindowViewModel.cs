using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Text.Json;
using ImageSorter.Models;
using ReactiveUI;

namespace ImageSorter.ViewModels;

public class MainWindowViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new RoutingState();

    public static JsonSerializerOptions JsonOptions { get; private set; } = new JsonSerializerOptions { WriteIndented = true };

    public AppState CurrentAppState { get; set; }

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

    [MemberNotNull(nameof(CurrentAppState))]
    private void FirstRunChecks()
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

                if (currentAppState is null)
                {
                    currentAppState = new AppState { AppStateFilePath = appStatePath };
                    File.WriteAllText(appStatePath, JsonSerializer.Serialize(currentAppState, JsonOptions));
                }
                else
                {
                    if (currentAppState.AppStateFilePath != appStatePath)
                    {
                        currentAppState.AppStateFilePath = appStatePath;
                    }
                }
            }
            // Handle bad json file
            catch (JsonException e)
            {
                currentAppState = new AppState { AppStateFilePath = appStatePath };
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

        this.CurrentAppState = currentAppState;
    }

    public MainWindowViewModel()
    {
        this.FirstRunChecks();

        if (this.CurrentAppState is not null
                && this.CurrentAppState.WindowHeight > 300
                && this.CurrentAppState.WindowWidth > 300)
        {
            this.WindowHeight = this.CurrentAppState.WindowHeight;
            this.WindowWidth = this.CurrentAppState.WindowWidth;
        }
        else
        {
            this.WindowHeight = 1000;
            this.WindowWidth = 1500;
        }

        Router.Navigate.Execute(new ProjectSelectionViewModel(CurrentAppState!, this, this.Router));

        this.WhenAnyValue(x => x.WindowHeight).Subscribe(_ => this.CurrentAppState!.WindowHeight = _);
        this.WhenAnyValue(x => x.WindowWidth).Subscribe(_ => this.CurrentAppState!.WindowWidth = _);
    }
}