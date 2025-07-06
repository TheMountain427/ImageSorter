using Avalonia.Layout;
using ReactiveUI;

namespace ImageSorter.Models;

public class VerticalToggles : ReactiveObject
{
    private VerticalAlignment _selectedOption;
    public VerticalAlignment SelectedOption
    {
        get { return _selectedOption; }
        set { this.RaiseAndSetIfChanged(ref _selectedOption, value); }
    }

    private VerticalOption _topOption = new VerticalOption("Top", "Move object to top bounds");
    public VerticalOption TopOption
    {
        get { return _topOption; }
        set { this.RaiseAndSetIfChanged(ref _topOption, value); }
    }

    private VerticalOption _centerOption = new VerticalOption("Center", "Move object to center of parent");
    public VerticalOption CenterOption
    {
        get { return _centerOption; }
        set { this.RaiseAndSetIfChanged(ref _centerOption, value); }
    }

    private VerticalOption _bottomOption = new VerticalOption("Bottom", "Move object to bottom bounds");
    public VerticalOption BottomOption
    {
        get { return _bottomOption; }
        set { this.RaiseAndSetIfChanged(ref _bottomOption, value); }
    }

    private VerticalOption _stretchOption = new VerticalOption("Stretch", "Streach object to bounds");
    public VerticalOption StretchOption
    {
        get { return _stretchOption; }
        set { this.RaiseAndSetIfChanged(ref _stretchOption, value); }
    }

    private bool _finished { get; set; } = true;

    private void HandleReentry(IReactivePropertyChangedEventArgs<IReactiveObject> e)
    {
        if (_finished)
        {
            HandleSingleSelections(e);
        }
    }

    private void HandleSingleSelections(IReactivePropertyChangedEventArgs<IReactiveObject> e)
    {
        if (e.Sender is AccessibleBool ab && ab.BooleanValue == true)
        {
            _finished = false;

            // lol
            // Smelly way to only allow one selection
            if (ab != TopOption.OptionSelection)
            {
                TopOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = VerticalAlignment.Top;
            }

            if (ab != CenterOption.OptionSelection)
            {
                CenterOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = VerticalAlignment.Center;
            }

            if (ab != BottomOption.OptionSelection)
            {
                BottomOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = VerticalAlignment.Bottom;
            }

            if (ab != StretchOption.OptionSelection)
            {
                StretchOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = VerticalAlignment.Stretch;
            }

            _finished = true;
        }
    }

    public void SetSelectedOption(VerticalAlignment VerticalOption)
    {
        switch (VerticalOption)
        {
            case VerticalAlignment.Top:
                TopOption.OptionSelection.BooleanValue = true;
                break;
            case VerticalAlignment.Center:
                CenterOption.OptionSelection.BooleanValue = true;
                break;
            case VerticalAlignment.Bottom:
                BottomOption.OptionSelection.BooleanValue = true;
                break;
            case VerticalAlignment.Stretch:
                StretchOption.OptionSelection.BooleanValue = true;
                break;
            default:
                break;
        }
    }

    public VerticalToggles()
    {
        this.TopOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
        this.CenterOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
        this.BottomOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
        this.StretchOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
    }
}