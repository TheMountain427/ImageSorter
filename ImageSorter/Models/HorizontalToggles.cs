

using Avalonia.Layout;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace ImageSorter.Models;

public class HorizontalToggles : ReactiveObject
{
    private HorizontalAlignment _selectedOption;
    public HorizontalAlignment SelectedOption
    {
        get { return _selectedOption; }
        set { this.RaiseAndSetIfChanged(ref _selectedOption, value); }
    }

    private HorizontalOption _leftOption = new HorizontalOption("Left", "Move object to left bounds");
    public HorizontalOption LeftOption
    {
        get { return _leftOption; }
        set { this.RaiseAndSetIfChanged(ref _leftOption, value); }
    }

    private HorizontalOption _centerOption = new HorizontalOption("Center", "Move object to center of parent");
    public HorizontalOption CenterOption
    {
        get { return _centerOption; }
        set { this.RaiseAndSetIfChanged(ref _centerOption, value); }
    }

    private HorizontalOption _rightOption = new HorizontalOption("Right", "Move object to right bounds");
    public HorizontalOption RightOption
    {
        get { return _rightOption; }
        set { this.RaiseAndSetIfChanged(ref _rightOption, value); }
    }

    private HorizontalOption _stretchOption = new HorizontalOption("Stretch", "Streach object to bounds");
    public HorizontalOption StretchOption
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
            if (ab != LeftOption.OptionSelection)
            {
                LeftOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = HorizontalAlignment.Left;
            }

            if (ab != CenterOption.OptionSelection)
            {
                CenterOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = HorizontalAlignment.Center;
            }

            if (ab != RightOption.OptionSelection)
            {
                RightOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = HorizontalAlignment.Right;
            }

            if (ab != StretchOption.OptionSelection)
            {
                StretchOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = HorizontalAlignment.Stretch;
            }

            _finished = true;
        }
    }

    public void SetSelectedOption(HorizontalAlignment HorizontalOption)
    {
        switch (HorizontalOption)
        {
            case HorizontalAlignment.Left:
                LeftOption.OptionSelection.BooleanValue = true;
                break;
            case HorizontalAlignment.Center:
                CenterOption.OptionSelection.BooleanValue = true;
                break;
            case HorizontalAlignment.Right:
                RightOption.OptionSelection.BooleanValue = true;
                break;
            case HorizontalAlignment.Stretch:
                StretchOption.OptionSelection.BooleanValue = true;
                break;
            default:
                break;
        }
    }

    public HorizontalToggles()
    {

        this.LeftOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
        this.CenterOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
        this.RightOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
        this.StretchOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
    }
}