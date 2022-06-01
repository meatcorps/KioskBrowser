using KioskBrowser.DataService.Interface;

namespace KioskBrowser.DataService.Data;

[Serializable]
public class MessageData: IGuidData
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
}