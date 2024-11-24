using System;
using System.Collections.Generic;
using static ImageSorter.Models.SortOptionKey;

namespace ImageSorter.Models
{

    public class SortConfigs
    {
        public List<ISortOption> SortOptions { get; set; }

        public string GetOptionText(SortOptionKey SortOptionKey)
        {
            foreach (var option in this.SortOptions)
            {
                if (option.OptionKey == SortOptionKey)
                {
                    return option.OptionText;
                }
            }

            throw new KeyNotFoundException();
        }

        public SortOptionKey GetOptionKey(string SortOptionText)
        {
            foreach (var option in this.SortOptions)
            {
                if (option.OptionText == SortOptionText)
                {
                    return option.OptionKey;
                }
            }

            throw new KeyNotFoundException();
        }

        public ISortOption this[string OptionText]
        {
            get => this.SortOptions.Find(x => x.OptionText == OptionText) ?? throw new KeyNotFoundException();
        }

        public ISortOption this[SortOptionKey SortOptionKey]
        {
            get => this.SortOptions.Find(x => x.OptionKey == SortOptionKey) ?? throw new KeyNotFoundException();
        }

        public void SetValue<T>(SortOptionKey SortOptionKey, T NewValue)
        {
            var option = SortOptions.Find(x => x.OptionKey == SortOptionKey);

            if (option is SortOption<T> typedOption)
            {
                typedOption.Value = NewValue;
            }
            else
            {
                throw new InvalidOperationException($"SortOption type does not match. {NewValue.GetType} != {option.GetType}");
            }
        }

        public void SetValue<T>(string SortOptionText, T NewValue)
        {
            var option = SortOptions.Find(x => x.OptionText == SortOptionText);

            if (option is SortOption<T> typedOption)
            {
                typedOption.Value = NewValue;
            }
            else
            {
                throw new InvalidOperationException($"SortOption type does not match. {NewValue.GetType} != {option.GetType}");
            }
        }

        public SortOption<T> GetKey<T>(SortOptionKey SortOptionKey)
        {
            // Find the option by key
            var option = SortOptions.Find(x => x.OptionKey == SortOptionKey);

            // Ensure the option is of type SortOption<T>
            if (option is SortOption<T> typedOption)
            {
                return typedOption;
            }

            throw new InvalidOperationException($"The SortOption type does not match the specified type.");
        }

        public SortOption<T> GetKey<T>(string SortOptionText)
        {
            // Find the option by key
            var option = SortOptions.Find(x => x.OptionText == SortOptionText);

            // Ensure the option is of type SortOption<T>
            if (option is SortOption<T> typedOption)
            {
                return typedOption;
            }

            throw new InvalidOperationException($"The SortOption type does not match the specified type.");
        }

        public bool ContainsKey(SortOptionKey sortOptionKey)
        {
            return SortOptions.Exists(x => x.OptionKey == sortOptionKey);
        }

        public bool ContainsText(string optionText)
        {
            return SortOptions.Exists(x => x.OptionText == optionText);
        }

        public SortConfigs()
        {
            this.SortOptions = new List<ISortOption>()
            {
                new SortOption<bool>("Ignore unsorted", IgnoreUnsorted, false),
                new SortOption<bool>("Include unsorted", IncludeUnsorted, false),
                new SortOption<bool>("Ignore orphans", IgnoreOrphans, false),
                new SortOption<bool>("Include orphans", IncludeOrphans, false)
            };
        }
    }
}
