using KioskBrowser.DataService.Interface;

namespace KioskBrowser.DataService.Data;

[Serializable]
public class GroupData: IGuidData
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int SortIndex { get; set; }
}