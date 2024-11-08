using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ImageSorter.Models
{
    public class ImageDetails
    {
        public string FileName { get; set; }
        public ulong FileSize { get; set; }
        public string FilePath { get; set; }
        public DateTimeOffset FileCreatedTime { get; set; }
        public DateTimeOffset FileLastModifiedTime { get; set; }
        public string FilteredValue { get; set; } = "Unsorted";
        public bool IsValid { get; set; } = true;

        [JsonIgnore]
        public Bitmap ImageBitmap { get; set; }
        //[JsonIgnore]
        //public bool ImageLoaded { get; set; }
        
        //[JsonIgnore]
        //private Bitmap _imageBitmap { get; }
        //[JsonIgnore]
        //public Bitmap ImageBitmap
        //{
        //    get => _imageBitmap;
        //    set
        //    {
        //        _imageBitmap = value;
        //        _onProjectConfigChange?.Invoke(this, EventArgs.Empty);
        //        this.SetLastModifiedTime();
        //    }
        //}

        public ImageDetails()
        {

        }

        public ImageDetails(string filePath)
        {
            CreateImageDetails(filePath);
        }

        private void CreateImageDetails(string filePath)
        {
            var file = Task.Run(() => App.TopLevel.StorageProvider.TryGetFileFromPathAsync(filePath));
            file.Wait();

            if (!file.IsCompletedSuccessfully || file.Result is null)
            {
                // File doesn't exist, so don't make it
                return;
            }


            var fileProperties = Task.Run(() => file.Result.GetBasicPropertiesAsync());
            fileProperties.Wait();

            if (!fileProperties.IsCompletedSuccessfully || fileProperties.Result is null)
            {
                throw new Exception("Explode");
            }

            this.FileName = file.Result.Name;
            this.FileCreatedTime = (DateTimeOffset)fileProperties.Result.DateCreated!;
            this.FileLastModifiedTime = (DateTimeOffset)fileProperties.Result.DateModified!;
            this.FileSize = (ulong)fileProperties.Result.Size!;
            this.FilePath = filePath;
            this.IsValid = true;
        }

        // This might not be needed anymore, at least not on project init
        public bool ValidateImageProperties()
        {
            var filePath = this.FilePath;
            var file = Task.Run(() => App.TopLevel.StorageProvider.TryGetFileFromPathAsync(filePath));
            file.Wait();

            if (!file.IsCompletedSuccessfully || file.Result is null)
            {
                return false;
            }

            var fileProperties = Task.Run(() => file.Result.GetBasicPropertiesAsync());
            fileProperties.Wait();

            if (!fileProperties.IsCompletedSuccessfully || fileProperties.Result is null)
            {
                return false;
            }

            this.FileName = file.Result.Name;
            this.FileCreatedTime = (DateTimeOffset)fileProperties.Result.DateCreated!;
            this.FileLastModifiedTime = (DateTimeOffset)fileProperties.Result.DateModified!;
            this.FileSize = (ulong)fileProperties.Result.Size!;
            this.FilePath = filePath;
            this.IsValid = true;

            return true;
        }
    }
}
