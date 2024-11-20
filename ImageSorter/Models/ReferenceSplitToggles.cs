
using static ImageSorter.Models.Enums;
using ReactiveUI;
using System;

namespace ImageSorter.Models;

public class ReferenceSplitToggles : ReactiveObject
{

    private ReferenceSplit _selectedOption;
    public ReferenceSplit SelectedOption
    {
        get { return _selectedOption; }
        set { this.RaiseAndSetIfChanged(ref _selectedOption, value); }
    }

    private ReferenceSplitOption _splitOption = new ReferenceSplitOption("Split", "Split reference images between reference views");
    public ReferenceSplitOption SplitOption
    {
        get { return _splitOption; }
        set { this.RaiseAndSetIfChanged(ref _splitOption, value); }
    }

    private ReferenceSplitOption _alphaOption = new ReferenceSplitOption("Alpha", "All reference images on alpha reference view");
    public ReferenceSplitOption AlphaOption
    {
        get { return _alphaOption; }
        set { this.RaiseAndSetIfChanged(ref _alphaOption, value); }
    }

    private ReferenceSplitOption _betaOption = new ReferenceSplitOption("Beta", "All reference images on beta reference view");
    public ReferenceSplitOption BetaOption
    {
        get { return _betaOption; }
        set { this.RaiseAndSetIfChanged(ref _betaOption, value); }
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
            if (ab != SplitOption.OptionSelection)
            {
                SplitOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = ReferenceSplit.Split;
            }

            if (ab != AlphaOption.OptionSelection)
            {
                AlphaOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = ReferenceSplit.Alpha;
            }

            if (ab != BetaOption.OptionSelection)
            {
                BetaOption.OptionSelection.BooleanValue = false;
            }
            else
            {
                SelectedOption = ReferenceSplit.Beta;
            }

            _finished = true;
        }
    }

    public void SetSelectedOption(ReferenceSplit ReferenceSplit)
    {
        switch (ReferenceSplit)
        {
            case ReferenceSplit.Split:
                SplitOption.OptionSelection.BooleanValue = true;
                break;
            case ReferenceSplit.Alpha:
                AlphaOption.OptionSelection.BooleanValue = true;
                break;
            case ReferenceSplit.Beta:
                BetaOption.OptionSelection.BooleanValue = true;
                break;
            default:
                break;
        }
    }
        
    public ReferenceSplitToggles()
    {
        this.SplitOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
        this.AlphaOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
        this.BetaOption.OptionSelection.Changed.Subscribe(_ => HandleReentry(_));
    }
}