using ImageSorter.Models;
using ReactiveUI;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class SortConfirmationViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "SortConfirmationViewModel";

    public List<SortConfirmation> SortConfirmations { get; }

    private bool _canContinue;
    public bool CanContinue
    {
        get { return _canContinue; }
        set { this.RaiseAndSetIfChanged(ref _canContinue, value); }
    }

    public AccessibleString ContinueButton { get; set; } = new AccessibleString("Continue");

    public ICommand CloseOverlay { get; }

    public ICommand? OnCancelCommand { get; }

    // uh this needs to always take a List<string>, not sure how to make that clear though
    public ICommand? OnSuccessCommand { get; }

    // Stinky, only contains SortConfirmations that require a selection
    private List<List<AccessibleBool>> _requiredOptionsSeperated { get; } = new List<List<AccessibleBool>>();

    public void ConfirmSelection()
    {
        // Get all option texts that are selected
        var selectedOptions = this.SortConfirmations.SelectMany(x => x.ConfirmationOptions.Where(x => x.OptionSelection.BooleanValue == true)
                                                                                          .Select(x => x.OptionText));

        OnSuccessCommand.Execute(selectedOptions);
        // OnSuccessCommand will close the view now
        //CloseOverlay.Execute(null);
    }

    private void UpdateCanContinue(IReactivePropertyChangedEventArgs<IReactiveObject> e)
    {
        // Fires muliple times when a SortConfirmation has AllowMultipleSelections == true
        // Should really affect performance because we basically cached the values we are checking
        if (e.Sender is AccessibleBool ab)
        {
            // Only do anything if there are required options
            if (this._requiredOptionsSeperated.Count > 0)
            {
                // Check each SortConfirmation to see if an option is checked
                // If any SortConfirmation does not have an option selected, prevent continue and fail fast
                foreach (var thing in _requiredOptionsSeperated)
                {
                    if (thing.Any(x => x.BooleanValue == true) == false)
                    {
                        CanContinue = false;
                        return;
                    }
                }
                // All SortConfirmations have something selected, can continue
                CanContinue = true;
            }
        }
    }

    public void DebugButton()
    {

    }

    public SortConfirmationViewModel(AppState CurrentAppState, IEnumerable<SortConfirmation> SortConfirmations, ICommand CloseOverlay)
                                     : base(CurrentAppState)
    {
        this.SortConfirmations = (List<SortConfirmation>)SortConfirmations;
        this.CloseOverlay = CloseOverlay;


        // This is uneeded but I am leaving it here for reference for now
        /////////////////////////////////////////////////////////////////////////////////////
        // Subscribes to boolean values and send the value of the boolean to a method
        // If you only care about the value, sends events on initalization too
        //this.SortConfirmations.Where(x => x.RequiredToContinue == true).SelectMany(x => x.ConfirmationOptions.Select(x =>x.OptionSelection))
        //.ToList().ForEach(x => x.WhenAnyValue(x => x.BooleanValue).Subscribe(_ => DebugButton(_)));

        // Subscribes to boolean values and sends IReactivePropertyChangedEventArgs<IReactiveObject> event
        // Sends the whole event args, only after initializtion
        //this.SortConfirmations.Where(x => x.RequiredToContinue == true).SelectMany(x => x.ConfirmationOptions.Select(x => x.OptionSelection))
        //.ToList().ForEach(x => x.Changed.Subscribe(_ => DebugButton(_)));
        /////////////////////////////////////////////////////////////////////////////////////



        // Gunna do some smelly code even though I don't have to,
        // learning oportunity for navigating these objects.
        // This is a way to subscribe to the lower OptionSelections changed events from a higher class

        // Stop some null references
        if (this.SortConfirmations.Count > 0)
        {
            // Find any confirmations that need a value set in order to continue
            // Stop if there are none
            var requiredConfirmations = this.SortConfirmations.Where(x => x.RequiredToContinue == true);
            if (requiredConfirmations.Any())
            {
                // Doing foreach so we can seperate AccessibleBools by SortConfirmation.
                // This should make later checks by UpdateCanContinue faster by having the options already seperated
                foreach (var sortConfirmation in requiredConfirmations)
                {
                    // List > Foreach so we can subscribe to each one
                    // I want it to break if options is null, means options were not added
                    var options = sortConfirmation.ConfirmationOptions.Select(x => x.OptionSelection).ToList();

                    // Add the options to a private list for UpdateCanContinue to check
                    this._requiredOptionsSeperated.Add(options);

                    // Subscribe to AccessibleBool change events
                    options.ForEach(x => x.Changed.Subscribe(_ => UpdateCanContinue(_)));
                }
            }
        }

        // If nothing is required that Continue button should be active
        if (_requiredOptionsSeperated.Count == 0)
        {
            CanContinue = true;
        }
    }

    public SortConfirmationViewModel(AppState CurrentAppState, IEnumerable<SortConfirmation> SortConfirmations,
                                     ICommand CloseOverlay, ICommand OnSuccessCommand, ICommand OnCancelCommand)
                                     : this (CurrentAppState, SortConfirmations, CloseOverlay)
    {
        this.OnSuccessCommand = OnSuccessCommand;
        this.OnCancelCommand = OnCancelCommand;
    }
}