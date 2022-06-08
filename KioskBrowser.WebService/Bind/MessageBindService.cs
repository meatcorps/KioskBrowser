using System.Reactive.Linq;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Services;
using KioskBrowser.WebService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace KioskBrowser.WebService.Bind;

public class MessageBindService: IDisposable
{
    private readonly IHubContext<DataHub> _dataHubContext;
    private readonly DataCollectionService _dataCollectionService;
    private readonly CancellationTokenRegistration _cancellationTokenRegistration = new ();

    public MessageBindService(IHubContext<DataHub> dataHubContext, DataCollectionService dataCollectionService)
    {
        _dataHubContext = dataHubContext;
        _dataCollectionService = dataCollectionService;
        _dataCollectionService
            .OnReady
            .Take(1)
            .Subscribe(_ =>
            {
                _dataCollectionService.MessageData!.OnAdded.Subscribe(OnChange,
                    _cancellationTokenRegistration.Token);
            });
    }

    private void OnChange(MessageData data)
    {
        _dataHubContext.Clients.All.SendAsync("IncomingMessage", data);
    }

    public void Dispose()
    {
        _cancellationTokenRegistration.Dispose();
        GC.SuppressFinalize(this);
    }
}