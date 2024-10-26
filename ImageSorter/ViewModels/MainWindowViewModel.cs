using DynamicData;
using ReactiveUI;
using System.Reactive;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class MainWindowViewModel : ReactiveObject, IScreen
{

    public RoutingState Router { get; } = new RoutingState();

    public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }

    public ReactiveCommand<Unit, IRoutableViewModel> GoBack => Router.NavigateBack;

    public MainWindowViewModel()
    {
        Router.Navigate.Execute(new ProjectSelectionViewModel(this, Router));
    }
}