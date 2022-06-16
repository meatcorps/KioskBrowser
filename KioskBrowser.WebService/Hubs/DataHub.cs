using KioskBrowser.DataService.Data;
using Microsoft.AspNetCore.SignalR;
using KioskBrowser.DataService.Services;
using KioskBrowser.WebService.Bind;

namespace KioskBrowser.WebService.Hubs;

public partial class DataHub: Hub
{
    protected readonly DataCollectionService DataService;
    protected readonly PhotoBindService PhotoBindService;

    public DataHub(DataCollectionService dataService, PhotoBindService photoBindService)
    {
        DataService = dataService;
        PhotoBindService = photoBindService;
    }

    
    
    
}