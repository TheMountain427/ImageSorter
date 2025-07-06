using ImageSorter.Models;
using ReactiveUI;
using System.Windows.Input;

namespace ImageSorter.ViewModels;

public class InProgressViewModel : ViewModelBase
{
    public override string UrlPathSegment { get; } = "InProgressView";

    public string? InProgressText { get; }

    public double ProgressMinimum { get; }

    public double ProgressMaximum { get; }

    public bool PauseOnCompletion { get; }

    public string? CompletionMessage { get; }

    public ICommand OnSuccessCommand { get; }

    // Value that goes up
    private double _currentProgressValue;
    public double CurrentProgressValue
    {
        get { return _currentProgressValue; }
        set { this.RaiseAndSetIfChanged(ref _currentProgressValue, value); }
    }

    // Are we done yet
    private bool _progressComplete;
    public bool ProgressComplete
    {
        get { return _progressComplete; }
        set { this.RaiseAndSetIfChanged(ref _progressComplete, value); }
    }

    // Toggle continue button and completion message if applicable
    private bool _showCompletionControls = false;
    public bool ShowCompletionControls
    {
        get { return _progressComplete; }
        set { this.RaiseAndSetIfChanged(ref _showCompletionControls, value); }
    }

    // Will toggle from InProgress message to Completion message if set up that way
    private string? _progressMessage;
    public string ProgressMessage
    {
        get { return _progressMessage ?? ""; }
        set { this.RaiseAndSetIfChanged(ref _progressMessage, value); }
    }

    // If complete, mark job as done. Will allow completion message/button to show if they are enabled
    private void CheckIfProgressComplete(double Value)
    {
        if (Value == ProgressMaximum)
        {
            ProgressComplete = true;
        }
    }

    // On complete, show buttons and message if PauseOnCompletion was set to true
    private void OnProgressComplete()
    {
        if (ProgressComplete == true)
        {
            if (PauseOnCompletion == true)
            {
                if (!string.IsNullOrEmpty(CompletionMessage))
                {
                    ProgressMessage = CompletionMessage;
                }

                ShowCompletionControls = true;
            }
            else
            {
                // If we aren't pausing when completition is done, run the success command provided
                this.OnSuccessCommand.Execute(null);
            }
        }
    }

    public void ContinueCommand()
    {
        this.OnSuccessCommand.Execute(null);
    }

    public void DebugCommand()
    {

    }

    public InProgressViewModel(AppState CurrentAppState, double MinimumValue, double MaximumValue,
                               IObservable<double> ValueToWatch, ICommand OnSuccessCommand) : base (CurrentAppState)
    {
        this.ProgressMinimum = MinimumValue;
        this.ProgressMaximum = MaximumValue;
        this.OnSuccessCommand = OnSuccessCommand;

        // Subscribe to the passed in IObservable<double> and set this.CurrentProgressValue equal to it
        // This lets us view value as it is changed by other view models
        ValueToWatch.Subscribe(x =>
        {
            this.CurrentProgressValue = x;
        });


        this.WhenAnyValue(x => x.CurrentProgressValue).Subscribe(_ => CheckIfProgressComplete(_));
        this.WhenAnyValue(x => x.ProgressComplete).Subscribe(_ => OnProgressComplete());
    }

    public InProgressViewModel(AppState CurrentAppState, double MinimumValue, double MaximumValue, IObservable<double> ValueToWatch, ICommand OnSuccessCommand, string InProgressText)
        : this(CurrentAppState, MinimumValue, MaximumValue, ValueToWatch, OnSuccessCommand)
    {
        this.InProgressText = InProgressText;
        this.ProgressMessage = this.InProgressText;
    }

    public InProgressViewModel(AppState CurrentAppState, double MinimumValue, double MaximumValue, IObservable<double> ValueToWatch, ICommand OnSuccessCommand, string InProgressText, bool PauseOnCompletion)
        : this(CurrentAppState, MinimumValue, MaximumValue, ValueToWatch, OnSuccessCommand, InProgressText)
    {
        this.PauseOnCompletion = PauseOnCompletion;
    }

    public InProgressViewModel(AppState CurrentAppState, double MinimumValue, double MaximumValue, IObservable<double> ValueToWatch, ICommand OnSuccessCommand, string InProgressText, bool PauseOnCompletion, string CompletionMessage)
        : this(CurrentAppState,MinimumValue, MaximumValue, ValueToWatch, OnSuccessCommand, InProgressText, PauseOnCompletion)
    {
        this.CompletionMessage = CompletionMessage;
    }

    public InProgressViewModel(AppState CurrentAppState, double MinimumValue, double MaximumValue, IObservable<double> ValueToWatch, ICommand OnSuccessCommand, bool PauseOnCompletion)
        : this(CurrentAppState,MinimumValue, MaximumValue, ValueToWatch, OnSuccessCommand)
    {
        this.PauseOnCompletion = PauseOnCompletion;
    }

    public InProgressViewModel(AppState CurrentAppState, double MinimumValue, double MaximumValue, IObservable<double> ValueToWatch, ICommand OnSuccessCommand, bool PauseOnCompletion, string CompletionMessage)
        : this(CurrentAppState, MinimumValue, MaximumValue, ValueToWatch, OnSuccessCommand, PauseOnCompletion)
    {
        this.CompletionMessage = CompletionMessage;
    }
}