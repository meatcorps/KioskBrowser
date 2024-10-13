using KioskBrowser.DataService.Interface;

namespace KioskBrowser.DataService.Data;

[Serializable]
public class ActionData : IGuidData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Action { get; set; } = "";
    public string Target { get; set; } = "";
}