using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace KioskBrowser.DataService.Services;

public class PhotoWatcherService : IDisposable
{
    private readonly string _targetFolder;
    private readonly string _urlFolder;
    private readonly FileSystemWatcher? _fileSystemWatcher;
    private readonly ReplaySubject<string> _newPhotoSubject = new ();
    public IObservable<string> NewPhoto => _newPhotoSubject
        .Where(x => x.ToLower().EndsWith(".jpg") || x.ToLower().EndsWith(".jpeg"))
        .Select(x => x.Replace(_targetFolder, _urlFolder))
        .AsObservable();

    public PhotoWatcherService(string targetFolder, string urlFolder)
    {
        _targetFolder = targetFolder;
        _urlFolder = urlFolder;

        if (!Directory.Exists(targetFolder))
        {
            Console.WriteLine($"Can't find folder: " + targetFolder);
            return;
        }

        _fileSystemWatcher = new FileSystemWatcher(targetFolder);
        Observable.Start(GetAllPhotos);
        _fileSystemWatcher.Created += (sender, args) => _newPhotoSubject.OnNext(args.FullPath);
        _fileSystemWatcher.EnableRaisingEvents = true;
    }

    private void GetAllPhotos()
    {
        if (_fileSystemWatcher is null)
            return;

        foreach (var file in Directory.GetFiles(_fileSystemWatcher.Path))
            _newPhotoSubject.OnNext(file);
        
    }

    public void Dispose()
    {
        _fileSystemWatcher?.Dispose();
        _newPhotoSubject.Dispose();
        GC.SuppressFinalize(this);
    }
}