using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageSorter.Models;

public class SortConfirmation : ReactiveObject
{
    public string WarningText { get; set; } = "WarningText";

    public List<ConfirmationOption> ConfirmationOptions { get; set; }

    public bool AllowMultipleSelections { get; } = false;

    public bool RequiredToContinue { get; } = false;

    // If only one boolean (checkbox) selection is allowed, when a box is already checked,
    // if another is checked, flip the other booleans to false.
    private void HandleSinglularSelection(IReactivePropertyChangedEventArgs<IReactiveObject> e)
    {
        // Only care if limiting to one selection
        if (AllowMultipleSelections == false )
        {
            if (e.Sender is AccessibleBool ab && ab.BooleanValue == true)
            {
                this.ConfirmationOptions.Select(x => x.OptionSelection)
                                        .Where(x => x.AccessibleText != ab.AccessibleText)
                                        .ToList()
                                        .ForEach(x => x.BooleanValue = false);
            }
        }
    }

    public SortConfirmation(string WarningText, IEnumerable<KeyValuePair<string,string>> Pairs, bool RequiredToContinue)
    {
        this.WarningText = WarningText;
        this.RequiredToContinue = RequiredToContinue;
        this.ConfirmationOptions = new List<ConfirmationOption>();
        foreach (var pair in Pairs)
        {
            // Observable collections are dumb so lets do reactive, like we are supposed to
            var confirmationOption = new ConfirmationOption(pair);

            // Subscribe to the boolean changed event
            confirmationOption.OptionSelection.Changed.Subscribe(_ => HandleSinglularSelection(_));
            this.ConfirmationOptions.Add(confirmationOption);
            
        }

    }
}