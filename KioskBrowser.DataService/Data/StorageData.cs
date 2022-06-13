using KioskBrowser.DataService.Interface;

namespace KioskBrowser.DataService.Data;

[Serializable]
public class StorageData: IGuidData
{
    public Guid Id  { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}