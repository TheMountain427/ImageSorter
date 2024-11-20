using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static ImageSorter.Models.Enums;

namespace ImageSorter.Models
{
    // Change json to opt in
    // https://stackoverflow.com/questions/31104335/ignore-base-class-properties-in-json-net-serialization
    public class ImageDetails : ReactiveObject
    {
        [JsonInclude]
        public string FileName { get; set; }
        public ulong FileSize { get; set; }
        [JsonInclude]
        public string FilePath { get; set; }
        public DateTimeOffset FileCreatedTime { get; set; }
        public DateTimeOffset FileLastModifiedTime { get; set; }

        private string _filteredValue = "Unsorted";
        [JsonInclude]
        public string FilteredValue
        {
            get { return _filteredValue; }
            set { this.RaiseAndSetIfChanged(ref _filteredValue, value); }
        }

        public bool IsValid { get; set; } = true;
        public bool ImageNotLoaded { get; set; } = false;
        public ReferenceViewIdentifier ReferenceViewID { get; set; }

        private int _imageIndex;
        public int ImageIndex
        {
            get { return _imageIndex; }
            set { this.RaiseAndSetIfChanged(ref _imageIndex, value); }
        }

        [JsonIgnore]
        public Bitmap ImageBitmap { get; set; }

        [JsonIgnore]
        private Bitmap _thumbnailBitmap;
        [JsonIgnore]
        public Bitmap ThumbnailBitmap
        {
            get { return _thumbnailBitmap; }
            set { this.RaiseAndSetIfChanged(ref _thumbnailBitmap, value); }
        }


        public Stream LoadImageBitmap()
        {
            if (File.Exists(this.FilePath))
            {
                return File.OpenRead(this.FilePath);
            }

            throw new ArgumentNullException();
        }

        public async Task LoadThumbnailAsync()
        {
            var imageStream = LoadImageBitmap();

            this.ThumbnailBitmap = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 175));
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
        // Using this to set image detail properties for reference images
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

        public ImageDetails()
        {

        }

        public ImageDetails(string filePath)
        {
            CreateImageDetails(filePath);
        }
    }
}
