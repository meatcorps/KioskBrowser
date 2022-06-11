using System.Reactive.Linq;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Services;
using KioskBrowser.WebService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace KioskBrowser.WebService.Bind;

public class GroupBindService: IDisposable
{
    private readonly IHubContext<DataHub> _dataHubContext;
    private readonly DataCollectionService _dataCollectionService;
    private readonly CancellationTokenRegistration _cancellationTokenRegistration = new ();

    public GroupBindService(IHubContext<DataHub> dataHubContext, DataCollectionService dataCollectionService)
    {
        _dataHubContext = dataHubContext;
        _dataCollectionService = dataCollectionService;
        _dataCollectionService
            .OnReady
            .Take(1)
            .Subscribe(_ =>
            {
                _dataCollectionService.GroupData!
                    .OnChangeList
                    .Sample(TimeSpan.FromSeconds(1))
                    .Subscribe(OnChangeList,
                    _cancellationTokenRegistration.Token);
            });
    }

    private void OnChangeList(GroupData[] data)
    {
        _dataHubContext.Clients.All.SendAsync("AllGroups", data);
    }

    public void Dispose()
    {
        _cancellationTokenRegistration.Dispose();
        GC.SuppressFinalize(this);
    }
}