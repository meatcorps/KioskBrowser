using System.Reactive.Disposables;
using System.Reactive.Linq;
using KioskBrowser.DataService.Services;
using KioskBrowser.WebService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace KioskBrowser.WebService.Bind;

public class PhotoBindService: IDisposable
{
    private readonly IHubContext<DataHub> _dataHubContext;
    private readonly PhotoWatcherService _photoWatcherService;
    private IDisposable _disposable = Disposable.Empty;

    public PhotoBindService(IHubContext<DataHub> dataHubContext, PhotoWatcherService photoWatcherService)
    {
        _dataHubContext = dataHubContext;
        _photoWatcherService = photoWatcherService;
    }

    public void StartListening()
    {
        _disposable.Dispose();
        _disposable = _photoWatcherService
            .NewPhoto
            .Subscribe(photo =>
            {
                _dataHubContext.Clients.Group("photo").SendAsync("newPhoto", photo);
            });
    }

    public void Dispose()
    {
        _photoWatcherService.Dispose();
        _disposable.Dispose();
        GC.SuppressFinalize(this);
    }
}