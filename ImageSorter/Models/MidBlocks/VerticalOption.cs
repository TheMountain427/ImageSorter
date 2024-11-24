using ReactiveUI;
using System.Collections.Generic;

namespace ImageSorter.Models;

public class VerticalOption : ReactiveObject
{
    public string OptionText { get; set; } = "OptionText";
    private AccessibleBool _optionSelection;
    public AccessibleBool OptionSelection
    {
        get { return _optionSelection; }
        set { this.RaiseAndSetIfChanged(ref _optionSelection, value); }
    }

    public VerticalOption(string OptionText, string AccessibleText)
    {
        this.OptionText = OptionText;
        this.OptionSelection = new AccessibleBool(AccessibleText);
    }

    public VerticalOption(KeyValuePair<string, string> Pair)
    {
        this.OptionText = Pair.Key;
        this.OptionSelection = new AccessibleBool(Pair.Value);
    }
}