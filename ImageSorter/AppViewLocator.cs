using ImageSorter.ViewModels;
using ImageSorter.Views;
using ReactiveUI;
using System;

namespace ImageSorter
{
    public class AppViewLocator : IViewLocator
    {
        public IViewFor ResolveView<T>(T viewModel, string contract = null) => viewModel switch
        {
            ProjectSelectionViewModel context => new ProjectSelectionView { DataContext = context },
            MainViewModel context => new MainView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}