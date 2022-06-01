using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Utilities;

namespace KioskBrowser.DataService.Services;

public class DataService: IDisposable
{
    private CollectionData _collectionData;
    private readonly Subject<Tuple<object, CollectionData>> _collectionDataSubject;
    private readonly CancellationTokenRegistration _disposeToken = new ();
    private CancellationTokenRegistration _crudDisposeToken = new ();
    private readonly Subject<Unit> _saveSubject = new();
    private readonly Subject<Unit> _onReadySubject = new();

    public CrudManager<GroupData>? GroupData { get; private set; }
    public CrudManager<MessageData>? MessageData { get; private set; }
    public CrudManager<ProductData>? ProductData { get; private set; }

    public IObservable<Unit> OnReady => _onReadySubject.AsObservable();

    public DataService(PersistentDataService saveLoadService)
    {
        _collectionDataSubject = saveLoadService.CollectionSubject;
        _collectionDataSubject
            .Where(x => x.Item1 != this)
            .Select(x => x.Item2)
            .Subscribe(x =>
            {
                _collectionData = x;
                SetCrudManagers();
            }, _disposeToken.Token);

        _saveSubject
            .Sample(TimeSpan.FromSeconds(1))
            .Where(x => _collectionData is not null)
            .Subscribe(_ => _collectionDataSubject
                    .OnNext(new Tuple<object, CollectionData>(this, _collectionData!)),
                _disposeToken.Token);
    }

    public void SetCrudManagers()
    {
        _crudDisposeToken.Dispose();
        _crudDisposeToken = new CancellationTokenRegistration();
        GroupData = new CrudManager<GroupData>(_collectionData.Groups);
        MessageData = new CrudManager<MessageData>(_collectionData.Messages);
        ProductData = new CrudManager<ProductData>(_collectionData.Products);

        GroupData.OnAdded.Subscribe(TriggerSave, _crudDisposeToken.Token);
        GroupData.OnChange.Subscribe(TriggerSave, _crudDisposeToken.Token);
        GroupData.OnRemoved.Subscribe(TriggerSave, _crudDisposeToken.Token);
        
        MessageData.OnAdded.Subscribe(TriggerSave, _crudDisposeToken.Token);
        MessageData.OnChange.Subscribe(TriggerSave, _crudDisposeToken.Token);
        MessageData.OnRemoved.Subscribe(TriggerSave, _crudDisposeToken.Token);

        ProductData.OnAdded.Subscribe(TriggerSave, _crudDisposeToken.Token);
        ProductData.OnChange.Subscribe(TriggerSave, _crudDisposeToken.Token);
        ProductData.OnRemoved.Subscribe(TriggerSave, _crudDisposeToken.Token);
        
        _onReadySubject.OnNext(Unit.Default);
    }

    public bool IsReady => GroupData is not null && MessageData is not null && ProductData is not null;
    
    private void TriggerSave(object newData)
    {
        _saveSubject.OnNext(Unit.Default);
    }

    public void Dispose()
    {
        _saveSubject.Dispose();
        _collectionDataSubject.Dispose();
        _disposeToken.Dispose();
        _onReadySubject.Dispose();
        _crudDisposeToken.Dispose();
        
        GC.SuppressFinalize(this);
    }
}