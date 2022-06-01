using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;
using KioskBrowser.DataService.Data;
using Newtonsoft.Json;

namespace KioskBrowser.DataService;

public class PersistentDataService: IDisposable
{
    private CollectionData _collectionData = new ();
    private FileInfo _file;

    private IDisposable _collectionUpdateSubscription;
    public Subject<Tuple<object,CollectionData>> CollectionSubject = new();

    public PersistentDataService(FileInfo file)
    {
        _file = file;
        _collectionUpdateSubscription = CollectionSubject
            .Where(x => x.Item1 != this)
            .Select(x => x.Item2)
            .Subscribe(x =>
            {
                _collectionData = x;
                Save();
            });
    }

    public void Load()
    {
        if (!_file.Exists)
        {
            _collectionData = new CollectionData();
            CollectionSubject.OnNext(new Tuple<object, CollectionData>(this, _collectionData));
            Save();
            return;
        }

        var jsonData = File.ReadAllText(_file.FullName);
        _collectionData = JsonConvert.DeserializeObject<CollectionData>(jsonData)!;
        CollectionSubject.OnNext(new Tuple<object, CollectionData>(this, _collectionData));
    }

    public void Save()
    {
        var jsonData = JsonConvert.SerializeObject(_collectionData);
        File.WriteAllText(_file.FullName, jsonData);
    }
    
    public void Dispose()
    {
        Save();
        CollectionSubject.Dispose();
        _collectionUpdateSubscription.Dispose();
    }
}