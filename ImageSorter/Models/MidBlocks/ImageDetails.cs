﻿using System.Diagnostics;
using System.Text.Json.Serialization;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ReactiveUI;
using SkiaSharp;
using static ImageSorter.Models.Enums;

namespace ImageSorter.Models;
// What a mess...
public class ImageDetails : ReactiveObject
{
    public string FileName { get; set; }
    public ulong FileSize { get; set; }
    public string FilePath { get; set; }
    public DateTimeOffset FileCreatedTime { get; set; }
    public DateTimeOffset FileLastModifiedTime { get; set; }
    public bool IsValid { get; set; } = true;
    public bool ImageNotLoaded { get; set; } = false;
    public ReferenceViewIdentifier ReferenceViewID { get; set; }

    public long ImageWidth { get; set; }
    public long ImageHeight { get; set; }
    public long ImageArea { get; set; }

    private string _filteredValue = "Unsorted";
    public string FilteredValue
    {
        get { return _filteredValue; }
        set { this.RaiseAndSetIfChanged(ref _filteredValue, value); }
    }

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

    private async Task CreateImageDetailsAsync(string filePath)
    {
        var file = await App.TopLevel.StorageProvider.TryGetFileFromPathAsync(filePath);

        if (file is null)
        {
            // File doesn't exist, so don't make it
            return;
        }

        var fileProperties = await file.GetBasicPropertiesAsync();

        if (fileProperties is null)
        {
            throw new Exception("Explode");
        }

        this.FileName = file.Name;
        this.FileCreatedTime = (DateTimeOffset)fileProperties.DateCreated!;
        this.FileLastModifiedTime = (DateTimeOffset)fileProperties.DateModified!;
        this.FileSize = (ulong)fileProperties.Size!;
        this.FilePath = filePath;
        this.IsValid = true;

        // https://github.com/mono/SkiaSharp/issues/1551
        using FileStream stream = new(filePath, FileMode.Open);
        using var skCodec = SKCodec.Create(stream);
        if (skCodec is not null)
        {
            this.ImageHeight = skCodec.Info.Height;
            this.ImageWidth = skCodec.Info.Width;
            this.ImageArea = ImageHeight * ImageWidth;
        }
        else
        {
            Debug.WriteLine($"Failed to create SKCodec for {filePath}. Image dimensions will be 0");
        }
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

        var skCodec = SKCodec.Create(filePath);

        this.ImageHeight = skCodec.Info.Height;
        this.ImageWidth = skCodec.Info.Width;
        this.ImageArea = ImageHeight * ImageWidth;

        return true;
    }

    public ImageDetails()
    {

    }

    private Task CreateImageDetails(string filePath)
        => Task.Run(() => CreateImageDetailsAsync(filePath));

    public ImageDetails(string filePath)
    {
        CreateImageDetails(filePath);
    }
}