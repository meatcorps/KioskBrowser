using KioskBrowser.DataService.Data;

namespace KioskBrowser.WebService.Hubs;

public partial class DataHub
{
    public void AddEditMessage(string id, string title, string message)
    {
        DataService.MessageData!.AddEdit(new MessageData
            {
                Id = id == "" ? Guid.NewGuid() : Guid.Parse(id),
                Title = title,
                Message = message
            }
        );
    }

    public void RemoveMessage(string id)
    {
        DataService.MessageData!.Delete(new MessageData
            {
                Id = Guid.Parse(id),
                Title = "",
                Message = ""
            }
        );
    }

    public IEnumerable<MessageData> AllMessages()
    {
        return DataService.MessageData!.All();
    } 
}