using KioskBrowser.DataService.Interface;

namespace KioskBrowser.DataService.Data;

[Serializable]
public class ProductData: IGuidData
{
    public Guid Id  { get; set; }
    public string Name { get; set; }
    public int SortIndex { get; set; }
    public Guid Group { get; set; }
    public int TotalItems { get; set; }
}