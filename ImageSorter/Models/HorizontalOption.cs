
using Avalonia.Layout;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace ImageSorter.Models;

public class HorizontalOption : ReactiveObject
{
    public string OptionText { get; set; } = "OptionText";
    private AccessibleBool _optionSelection;
    public AccessibleBool OptionSelection
    {
        get { return _optionSelection; }
        set { this.RaiseAndSetIfChanged(ref _optionSelection, value); }
    }

    public HorizontalOption(string OptionText, string AccessibleText)
    {
        this.OptionText = OptionText;
        this.OptionSelection = new AccessibleBool(AccessibleText);
    }

    public HorizontalOption(KeyValuePair<string, string> Pair)
    {
        this.OptionText = Pair.Key;
        this.OptionSelection = new AccessibleBool(Pair.Value);
    }
}