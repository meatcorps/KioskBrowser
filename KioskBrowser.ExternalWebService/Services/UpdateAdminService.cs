using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace KioskBrowser.ExternalWebService.Services;

public class UpdateAdminService: IDisposable
{
    private readonly VerifyService _verifyService;
    private readonly PushService _pushService;
    private readonly CancellationDisposable _cancellationDisposable = new();

    public UpdateAdminService(VerifyService verifyService, MessageService messageService, SaveIncomingImagesService saveIncomingImagesService, PushService pushService)
    {
        _verifyService = verifyService;
        _pushService = pushService;

        Observable.Merge(
            messageService.IncomingMessage.Select(x => x.Code),
            saveIncomingImagesService.ReadyToVerify.Select(x => x.Code))
            .Throttle(TimeSpan.FromSeconds(10))
            .Subscribe(Update, _cancellationDisposable.Token);
    }

    private void Update(string code)
    {
        _pushService.Send("New messages to verify!", $"Messages: {_verifyService.TotalMessages(code)} Pictures: {_verifyService.TotalPictures(code)}");
    }

    public void Dispose()
    {
        _cancellationDisposable.Dispose();
    }
}