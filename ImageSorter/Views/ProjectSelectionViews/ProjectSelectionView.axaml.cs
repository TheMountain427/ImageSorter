using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData.Binding;
using ImageSorter.ViewModels;
using ReactiveUI;

namespace ImageSorter.Views;

public partial class ProjectSelectionView : ReactiveUserControl<ProjectSelectionViewModel>
{
    public ProjectSelectionView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    public void ProjectSelection_ImgPath_TextBox_LostFocus(object sender ,RoutedEventArgs e)
    {
        if (DataContext is ProjectSelectionViewModel vm)
        {
            vm.ImgPathTextBoxLostFocus = true;
        }
    }

    public void ProjectSelection_ImgPath_TextBox_GotFocus(object sender ,GotFocusEventArgs e)
    {
        if (DataContext is ProjectSelectionViewModel vm)
        {
            vm.ImgPathTextBoxLostFocus = false;
        }
    }

    public void ProjectSelection_OutPath_TextBox_LostFocus(object sender ,RoutedEventArgs e)
    {
        if (DataContext is ProjectSelectionViewModel vm)
        {
            vm.OutPathTextBoxLostFocus = true;
        }
    }

    public void ProjectSelection_OutPath_TextBox_GotFocus(object sender ,GotFocusEventArgs e)
    {
        if (DataContext is ProjectSelectionViewModel vm)
        {
            vm.OutPathTextBoxLostFocus = false;
        }
    } 
}