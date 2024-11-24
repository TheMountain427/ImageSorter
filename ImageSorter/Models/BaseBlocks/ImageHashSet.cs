using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

                if (imageDetail is not null)
                {
                    imageDetail.FileName = string.IsNullOrEmpty(e.Name) ? imageDetail.FileName : e.Name;
                    imageDetail.FilePath = string.IsNullOrEmpty(e.FullPath) ? imageDetail.FilePath : e.FullPath;
                }
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
    }
}
