using KioskBrowser.Core.Service;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Services;

namespace KioskBrowser.WebService.Services;

public class FakeBlackoutService
{
    private readonly KioskMqttClient _client;
    private readonly DataCollectionService _dataCollectionService;

    public FakeBlackoutService(KioskMqttClient client, DataCollectionService dataCollectionService)
    {
        _client = client;
        _dataCollectionService = dataCollectionService;

        _client.Connected.Subscribe(_ =>
        {
            Task.Run(async () =>
            {
                (await _client.SubscribeToTopic("blackout")).Subscribe(data =>
                {
                    _dataCollectionService.StorageData?.AddEdit(new StorageData
                    {
                        Key = "blackout",
                        Value = data == "ON" ? "ON" : "OFF"
                    });
                });
            });
        });
    }
}