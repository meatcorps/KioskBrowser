using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using KioskBrowser.DataService.Data;

namespace KioskBrowser.ExternalWebService.Services;

public sealed class TransferService: IDisposable
{
    public int ChunkSize { get; }

    public IObservable<TransferData> NewCompletedTransfer =>
        _newCompletedTransferSubject.AsObservable();
    
    private readonly Subject<TransferData> _newCompletedTransferSubject = new();
    private readonly Dictionary<Guid, TransferData> _transfers = new();
    private readonly CancellationDisposable _cancellationDisposable = new();
    private readonly object _transferLock = new ();

    public TransferService(int chunkSize)
    {
        ChunkSize = chunkSize;
        Observable.Interval(TimeSpan.FromMinutes(10)).Subscribe(CheckForForgottenTransfers, _cancellationDisposable.Token);
    }

    private void CheckForForgottenTransfers(long _)
    {
        var now = DateTime.Now;
        var forgottenTransfers = _transfers.Values.Where(transfer => now - transfer.ChangeDate > TimeSpan.FromMinutes(20));
        
        lock (_transferLock)
        {
            foreach (var toRemove in forgottenTransfers)
                _transfers.Remove(toRemove.Id, out var _);
        }
    }

    public Guid NewBase64Transfer(string code, string name, int transferSize, string metaData)
    {
        var guid = Guid.NewGuid();
        var totalChunks = (int) Math.Ceiling((double) transferSize / ChunkSize);
        
        lock (_transferLock)
        {
            _transfers.Add(guid, new TransferData
            {
                Id = guid,
                Name = name,
                Code = code,
                TotalChunks = totalChunks,
                ChangeDate = DateTime.Now,
                TransferSize = transferSize,
                MetaData = metaData
            });
        }

        return guid;
    }

    public int TotalChunks(Guid id)
    {
        lock (_transferLock)
        {
            if (!_transfers.ContainsKey(id))
                return 0;

            return _transfers[id].TotalChunks - 1;
        }
    }
    
    public int SetData(Guid id, int chunk, string data)
    {
        lock (_transferLock)
        {
            if (!_transfers.ContainsKey(id))
                return -2;

            if (chunk >= _transfers[id].TotalChunks)
                return -3;

            _transfers[id].ChangeDate = DateTime.Now;
            _transfers[id].Chunks.Write(data);
            
            if (chunk <= _transfers[id].TotalChunks - 2)
                return chunk + 1;

            _transfers.Remove(id, out var transferData);
            
            _newCompletedTransferSubject.OnNext(transferData!);
            
            return -1;
        }
    }

    public void Dispose()
    {
        _newCompletedTransferSubject.Dispose();
        _cancellationDisposable.Dispose();
    }
}