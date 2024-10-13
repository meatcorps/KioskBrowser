using System.Reactive.Linq;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Services;
using KioskBrowser.WebService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace KioskBrowser.WebService.Bind;

public class ActionBindService: IDisposable
{
    private readonly IHubContext<DataHub> _dataHubContext;
    private readonly DataCollectionService _dataCollectionService;
    private readonly CancellationTokenRegistration _cancellationTokenRegistration = new ();

    public ActionBindService(IHubContext<DataHub> dataHubContext, DataCollectionService dataCollectionService)
    {
        _dataHubContext = dataHubContext;
        _dataCollectionService = dataCollectionService;
        _dataCollectionService
            .OnReady
            .Take(1)
            .Subscribe(_ =>
            {
                _dataCollectionService.ActionData!
                    .OnChangeList
                    .Sample(TimeSpan.FromSeconds(1))
                    .Subscribe(OnChangeList,
                        _cancellationTokenRegistration.Token);
            });
    }

    private void OnChangeList(ActionData[] data)
    {
        _dataHubContext.Clients.All.SendAsync("AllActions", data);
    }

    public void Dispose()
    {
        _cancellationTokenRegistration.Dispose();
        GC.SuppressFinalize(this);
    }
}