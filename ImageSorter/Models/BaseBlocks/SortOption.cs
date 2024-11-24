
namespace ImageSorter.Models
{    
    public enum SortOptionKey
    {
        IgnoreUnsorted = 0,
        IncludeUnsorted = 1,
        IgnoreOrphans = 2,
        IncludeOrphans = 3
    }

    public interface ISortOption
    {
        string OptionText { get; }
        SortOptionKey OptionKey { get; }
    }

    public class SortOption<T> : ISortOption
    {
        public string OptionText { get; }
        public SortOptionKey OptionKey { get; }
        public T Value { get; set; }

        public SortOption(string Option, SortOptionKey OptionKey, T Value)
        {
            this.OptionText = Option;
            this.OptionKey = OptionKey;
            this.Value = Value;
        }
    }
}
