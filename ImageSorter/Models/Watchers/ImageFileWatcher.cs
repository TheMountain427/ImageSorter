namespace ImageSorter.Models.Watchers;

public class ImageFileWatcher
{
    // ToDo: Actually implement this
    public FileSystemWatcher ImageWatcher { get; protected set; }

    public ImageFileWatcher(string path)
    {
        ImageWatcher = new FileSystemWatcher()
        {
            Path = path,
            //NotifyFilter = NotifyFilters.FileName,
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };

        //ImageWatcher.Changed += this.OnChanged;
        //ImageWatcher.Created += this.OnCreated;
        //ImageWatcher.Deleted += this.OnDeleted;
        //ImageWatcher.Renamed += this.OnRenamed;
        //ImageWatcher.Error += this.OnError;
    }
}