using KioskBrowser.ExternalWebService.Services;
using Microsoft.AspNetCore.SignalR;
using WebPush;

namespace KioskBrowser.ExternalWebService.Hubs;

public class DataHub : Hub
{
    private readonly CodeService _codeService;
    private readonly MessageService _messageService;
    private readonly VerifyService _verifyService;
    private readonly PushService _pushService;

    public DataHub(CodeService codeService, MessageService messageService, VerifyService verifyService, PushService pushService)
    {
        _codeService = codeService;
        _messageService = messageService;
        _verifyService = verifyService;
        _pushService = pushService;
    }
    
    public void SendMessage(string code, string name, string message)
    {
        if (!_codeService.IsValidCode(code))
            return;
        
        _messageService.SendMessage(code, name, message);
    }
    
    public bool ValidCode(string code)
    {
        return _codeService.IsValidCode(code);
    }
    
    public bool AdminCode(string code)
    {
        return _codeService.IsAdmin(code);
    }
    
    public int TotalToVerifyMessage(string code)
    {
        if (!_codeService.IsValidCode(code))
            return 0;

        return _verifyService.TotalMessages(code);
    }
    
    public int TotalToVerifyPicture(string code)
    {
        if (!_codeService.IsValidCode(code))
            return 0;

        return _verifyService.TotalPictures(code);
    }

    public void AddPushSubscription(string endpoint, string p256dh, string auth)
    {
        _pushService.Register(new PushSubscription(endpoint, p256dh, auth));
        _pushService.Send("Test message", "Successful registered new device for push messages");
    }

    public string GetPublicKey()
    {
        return _pushService.PublicKey;
    }
}