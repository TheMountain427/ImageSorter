using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter.Models
{
    public class ImageHashSet : HashSet<ImageDetails>
    {

        public IOrderedEnumerable<ImageDetails> SortByFileNameAscending()
        {
            return this.OrderBy(x => x.FileName);
        }

        public IOrderedEnumerable<ImageDetails> SortByFileNameDescending()
        {
            return this.OrderByDescending(x => x.FileName);
        }

        public IOrderedEnumerable<ImageDetails> SortByFileSizeAsceding()
        {
            return this.OrderBy(x => x.FileSize);
        }

        public IOrderedEnumerable<ImageDetails> SortByFileSizeDescending()
        {
            return this.OrderByDescending(x => x.FileSize);
        }

        public IOrderedEnumerable<ImageDetails> SortByFileCreationTimeAscending()
        {
            return this.OrderBy(x => x.FileCreatedTime);
        }

        public IOrderedEnumerable<ImageDetails> SortbyFileCreationTimeDescending()
        {
            return this.OrderByDescending(x => x.FileCreatedTime);
        }

        public IOrderedEnumerable<ImageDetails> SortByLastModifiedTimeAscending()
        {
            return this.OrderBy(x => x.FileLastModifiedTime);
        }

        public IOrderedEnumerable<ImageDetails> SortByLastModifiedTimeDescending()
        {
            return this.OrderByDescending(x => x.FileLastModifiedTime);
        }

        public IEnumerable<ImageDetails>? GetFilteredImages(string FilteredValue)
        {
            return this.Where(x => x.FilteredValue == FilteredValue);
        }

    }
}
