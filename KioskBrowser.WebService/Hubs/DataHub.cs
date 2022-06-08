using KioskBrowser.DataService.Data;
using Microsoft.AspNetCore.SignalR;
using KioskBrowser.DataService.Services;

namespace KioskBrowser.WebService.Hubs;

public partial class DataHub: Hub
{
    protected readonly DataCollectionService DataService;

    public DataHub(DataCollectionService dataService)
    {
        DataService = dataService;
    }

    
    
    
}