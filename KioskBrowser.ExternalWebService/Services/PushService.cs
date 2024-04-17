using Newtonsoft.Json;
using WebPush;

namespace KioskBrowser.ExternalWebService.Services;

public class PushService
{
    private readonly string _privateKey;
    public string PublicKey { get; }

    private readonly List<PushSubscription> Subscriptions = new();

    public PushService(string privateKey, string publicKey)
    {
        _privateKey = privateKey;
        PublicKey = publicKey;
    }
    
    public void Register(PushSubscription subscription)
    {
        Subscriptions.Add(subscription);
    }
    
    public void Send(string title, string body)
    {
        var webPushClient = new WebPushClient();
        var vapidDetails = new VapidDetails("mailto:example@example.com", PublicKey, _privateKey);
        
        foreach (var pushSubscription in Subscriptions)
            webPushClient.SendNotification(pushSubscription, GenerateJsonPayLoad(title, body), vapidDetails);
    }

    private string GenerateJsonPayLoad(string title, string body)
    {
        var dictionaryObject = new Dictionary<string, Dictionary<string, string>>();
        
        dictionaryObject.Add("notification", new Dictionary<string, string>());
        dictionaryObject["notification"].Add("title", title);
        dictionaryObject["notification"].Add("body", body);

        return JsonConvert.SerializeObject(dictionaryObject);
    }
}