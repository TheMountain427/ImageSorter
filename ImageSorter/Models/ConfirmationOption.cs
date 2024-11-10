

using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ImageSorter.Models;

public class ConfirmationOption : ReactiveObject
{
    public string OptionText { get; set; } = "OptionText";
    private AccessibleBool _optionSelection;
    public AccessibleBool OptionSelection
    {
        get { return _optionSelection; }
        set { this.RaiseAndSetIfChanged(ref _optionSelection, value); }
    }

    public ConfirmationOption(string optionText, string accessText)
    {
        this.OptionText = optionText;
        this.OptionSelection = new AccessibleBool(accessText);
    }

    public ConfirmationOption(KeyValuePair<string, string> pair)
    {
        this.OptionText = pair.Key;
        this.OptionSelection = new AccessibleBool(pair.Value);
    }
}