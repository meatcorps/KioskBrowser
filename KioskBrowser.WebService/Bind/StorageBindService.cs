using System.Reactive.Linq;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Services;
using KioskBrowser.WebService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace KioskBrowser.WebService.Bind;

public class StorageBindService: IDisposable
{
    private readonly IHubContext<DataHub> _dataHubContext;
    private readonly DataCollectionService _dataCollectionService;
    private readonly CancellationTokenRegistration _cancellationTokenRegistration = new ();

    public StorageBindService(IHubContext<DataHub> dataHubContext, DataCollectionService dataCollectionService)
    {
        _dataHubContext = dataHubContext;
        _dataCollectionService = dataCollectionService;
        _dataCollectionService
            .OnReady
            .Take(1)
            .Subscribe(_ =>
            {
                _dataCollectionService.StorageData!.OnAdded.Subscribe(OnChange,
                    _cancellationTokenRegistration.Token);
                _dataCollectionService.StorageData!.OnChange.Subscribe(OnChange,
                    _cancellationTokenRegistration.Token);
                _dataCollectionService.StorageData!.OnRemoved.Subscribe(OnRemoved,
                    _cancellationTokenRegistration.Token);
            });
    }

    private void OnChange(StorageData data)
    {
        _dataHubContext.Clients.All.SendAsync("StorageChange", data);
    }
    
    private void OnRemoved(StorageData data)
    {
        _dataHubContext.Clients.All.SendAsync("StorageRemove", data);
    }

    public void Dispose()
    {
        _cancellationTokenRegistration.Dispose();
        GC.SuppressFinalize(this);
    }
}