using KioskBrowser.Core.Service;
using KioskBrowser.DataService.Data;
using Microsoft.AspNetCore.SignalR;
using KioskBrowser.DataService.Services;
using KioskBrowser.WebService.Bind;

namespace KioskBrowser.WebService.Hubs;

public partial class DataHub: Hub
{
    protected readonly DataCollectionService DataService;
    protected readonly PhotoBindService PhotoBindService;
    protected readonly KioskMqttClient MqttClient;

    public DataHub(DataCollectionService dataService, PhotoBindService photoBindService, KioskMqttClient mqttClient)
    {
        DataService = dataService;
        PhotoBindService = photoBindService;
        MqttClient = mqttClient;
    }
    
}