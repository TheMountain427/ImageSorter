

using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ImageSorter.Models;

public class SortConfirmation : ReactiveObject
{
    public string WarningText { get; set; } = "WarningText";

    public List<ConfirmationOption> ConfirmationOptions { get; set; }

    public bool AllowMultipleSelections { get; } = false;

    private void HandleSinglularSelection(IReactivePropertyChangedEventArgs<IReactiveObject> e)
    {
        // Only care if limiting to one selection
        if (AllowMultipleSelections == false)
        {
            // This event action should be when bool flips value
            //if (e.Action == NotifyCollectionChangedAction.Replace ) // && action is replacing boolean?
            //{
                // To be continued...
            //}
        }

    }

    public SortConfirmation(string warningText, IEnumerable<KeyValuePair<string,string>> pairs)
    {
        this.WarningText = warningText;
        this.ConfirmationOptions = new List<ConfirmationOption>();
        foreach (var pair in pairs)
        {
            // Observable collections are dumb so lets do reactive, like we are supposed to
            var fuck = new ConfirmationOption(pair);

            // Subscrive to the boolean changed event
            fuck.OptionSelection.Changed.Subscribe(_ => HandleSinglularSelection(_));
            this.ConfirmationOptions.Add(fuck);
            
        }

    }
}