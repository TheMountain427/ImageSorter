

using ReactiveUI;
using System.Collections.Generic;

namespace ImageSorter.Models
{
    public class AccessibleBool : ReactiveObject
    {
        private bool _boolean;
        public bool Boolean
        {
            get { return _boolean; }
            set { this.RaiseAndSetIfChanged(ref _boolean, value); }
        }

        public string AccessText { get; set; }

        public AccessibleBool(string accessText)
        {
            this.AccessText = accessText;
        }

        public AccessibleBool(string accessText, bool boolean)
        {
            this.AccessText = accessText;
            this.Boolean = boolean;
        }
    }
}
