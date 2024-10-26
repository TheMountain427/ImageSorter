using ReactiveUI;

namespace ImageSorter.ViewModels;

public class MainViewModel : ReactiveObject, IRoutableViewModel
{
    
    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = "MainView";

    private string _greeting = "Welcome to Avalonia!";
    public string Greeting
    {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);

    }

    public void BtnCommand()
    {
        Greeting = "butthole";
    }
    public MainViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
