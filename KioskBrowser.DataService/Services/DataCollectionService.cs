using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Interface;
using KioskBrowser.DataService.Utilities;

namespace KioskBrowser.DataService.Services;

public class DataCollectionService: IDisposable
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
    public CrudManager<StorageData>? StorageData { get; private set; }
    public CrudManager<ActionData>? ActionData { get; private set; }

    public IObservable<Unit> OnReady => IsReady 
        ? Observable.Return(Unit.Default) 
        : _onReadySubject.AsObservable();

    public DataCollectionService(PersistentDataService saveLoadService)
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
        StorageData = new CrudManager<StorageData>(_collectionData.Storage);
        ActionData = new CrudManager<ActionData>(_collectionData.Actions);

        SubScribeToAllToTriggerSave(GroupData);
        SubScribeToAllToTriggerSave(MessageData);
        SubScribeToAllToTriggerSave(ProductData);
        SubScribeToAllToTriggerSave(StorageData);
        SubScribeToAllToTriggerSave(ActionData);
        
        _onReadySubject.OnNext(Unit.Default);
    }

    private void SubScribeToAllToTriggerSave<T>(CrudManager<T> crudManager) where T: IGuidData
    {
        crudManager.OnAdded.Subscribe(_ => TriggerSave(), _crudDisposeToken.Token);
        crudManager.OnChange.Subscribe(_ => TriggerSave(), _crudDisposeToken.Token);
        crudManager.OnRemoved.Subscribe(_ => TriggerSave(), _crudDisposeToken.Token);
    } 

    public bool IsReady => GroupData is not null && MessageData is not null && ProductData is not null;
    
    private void TriggerSave()
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