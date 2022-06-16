using KioskBrowser.DataService.Data;

namespace KioskBrowser.WebService.Hubs;

public partial class DataHub
{
    public async Task StartPhotoStream()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "photo");
        PhotoBindService.StartListening();
    }
}