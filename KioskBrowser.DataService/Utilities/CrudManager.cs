using System.Reactive.Linq;
using System.Reactive.Subjects;
using KioskBrowser.DataService.Interface;

namespace KioskBrowser.DataService.Utilities;

public class CrudManager<T> : IDisposable where T : IGuidData
{
    private readonly IList<T> _dataTable;

    public IObservable<T[]> OnChangeList => _onChangeListSubject.AsObservable(); 
    public IObservable<T> OnRemoved => _onRemovedSubject.AsObservable(); 
    public IObservable<T> OnAdded => _onAddedSubject.AsObservable(); 
    public IObservable<T> OnChange => _onChangeSubject.AsObservable(); 

    private readonly Subject<T[]> _onChangeListSubject = new();
    private readonly Subject<T> _onRemovedSubject = new();
    private readonly Subject<T> _onAddedSubject = new();
    private readonly Subject<T> _onChangeSubject = new();
    
    public CrudManager(IList<T> dataTable)
    {
        _dataTable = dataTable;
    }

    public void AddEdit(T item)
    {
        var oldItem = Get(item.Id);
        
        if (oldItem == null)
        {
            _dataTable.Add(item);
            _onAddedSubject.OnNext(item);
            TriggerUpdate();
            return;
        }
        
        _dataTable[Index(oldItem)] = item;
        _onChangeSubject.OnNext(item);
        TriggerUpdate();
    }

    public void Delete(T item)
    {
        var realItem = Get(item.Id);
        
        if (realItem is null)
            return;
        
        var index = Index(realItem);
        if (index == -1)
            return;
        
        _dataTable.RemoveAt(index);
        _onRemovedSubject.OnNext(realItem);
        TriggerUpdate();
    }

    public T? Get(Guid id)
    {
        try
        {
            return _dataTable
                .First(x => x.Id == id);
        }
        catch (InvalidOperationException e)
        {
            return default;
        }
    }

    public T[] All()
    {
        return _dataTable.ToArray();
    }
    
    private int Index(T item)
    {
        return _dataTable.IndexOf(item);
    }

    private void TriggerUpdate()
    {
        _onChangeListSubject.OnNext(All());
    }

    public void Dispose()
    {
        _onChangeListSubject.Dispose();
        _onRemovedSubject.Dispose();
        _onAddedSubject.Dispose();
        _onChangeSubject.Dispose();
    }
}