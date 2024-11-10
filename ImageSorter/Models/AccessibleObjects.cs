

using Avalonia.Controls.Primitives;
using ReactiveUI;
using System.Collections.Generic;

namespace ImageSorter.Models
{
    public class AccessibleBool : ReactiveObject
    {
        private bool _booleanValue;
        public bool BooleanValue
        {
            get { return _booleanValue; }
            set { this.RaiseAndSetIfChanged(ref _booleanValue, value); }
        }

        public string AccessibleText { get; set; }

        public AccessibleBool(string accessText)
        {
            this.AccessibleText = accessText;
        }

        public AccessibleBool(bool Boolean, string AccessibleText)
        {
            this.BooleanValue = Boolean;
            this.AccessibleText = AccessibleText;
        }
    }

    public class AccessibleString : ReactiveObject
    {
        private string _stringValue;
        public string StringValue
        {
            get { return _stringValue; }
            set { this.RaiseAndSetIfChanged(ref _stringValue, value); }
        }

        public string AccessibleText { get; set; }

        public AccessibleString(string StringValue)
        {
            this.StringValue = StringValue;
            this.AccessibleText = "";
        }

        public AccessibleString(string StringValue, string AccessibleText)
        {
            this.StringValue = StringValue;
            this.AccessibleText = AccessibleText;
        }
    }
}
