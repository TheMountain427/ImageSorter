using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter.Models
{
    public class ImageHashSet : HashSet<ImageDetails>
    {
        public void OnImageRenamed(object sender, RenamedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.OldName))
            {
                var imageDetail = this.FirstOrDefault(x => x.FileName == e.OldName);

                if (imageDetail is null && e.OldFullPath is not null)
                {
                    imageDetail = this.FirstOrDefault(x => x.FilePath == e.OldFullPath);

                    if (imageDetail is null)
                    {
                        throw new NotImplementedException();
                    }
                }
                else if (imageDetail is null && e.OldFullPath is null)
                {
                    throw new NotImplementedException();
                }


                imageDetail.FileName = string.IsNullOrEmpty(e.Name) ? imageDetail.FileName : e.Name;
                imageDetail.FilePath = string.IsNullOrEmpty(e.FullPath) ? imageDetail.FilePath : e.FullPath;
            }
        }

        public void OnImageDeleted(object sender, FileSystemEventArgs e)
        {
            var imageDetail = this.FirstOrDefault(x => x.FilePath == e.FullPath);

            if (imageDetail is not null)
            {
                this.Remove(imageDetail);
            }
        }

        public void OnImageCreated(object sender, FileSystemEventArgs e)
        {
            this.Add(new ImageDetails(e.FullPath));
        }

        public IOrderedEnumerable<ImageDetails> SortByFileNameAscending()
        {
            return this.OrderBy(x => x.FileName);
        }

        public IOrderedEnumerable<ImageDetails> SortByFileNameDescending()
        {
            return this.OrderByDescending(x => x.FileName);
        }

        public IOrderedEnumerable<ImageDetails> SortByFileSizeAscending()
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

        public IOrderedEnumerable<ImageDetails> SortByFileCreationTimeDescending()
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
