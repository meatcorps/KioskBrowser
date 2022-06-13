using KioskBrowser.DataService.Data;

namespace KioskBrowser.WebService.Hubs;

public partial class DataHub
{
    public void AddEditMessage(string id, string title, string message, string type)
    {
        DataService.MessageData!.AddEdit(new MessageData
            {
                Id = id == "" ? Guid.NewGuid() : Guid.Parse(id),
                Title = title,
                Message = message,
                Type = type
            }
        );
    }

    public void RemoveMessage(string id)
    {
        DataService.MessageData!.Delete(new MessageData
            {
                Id = Guid.Parse(id),
                Title = "",
                Message = "",
                Type = ""
            }
        );
    }

    public IEnumerable<MessageData> AllMessages()
    {
        return DataService.MessageData!.All();
    } 
}