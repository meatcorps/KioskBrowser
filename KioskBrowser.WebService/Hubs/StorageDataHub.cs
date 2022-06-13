using KioskBrowser.DataService.Data;

namespace KioskBrowser.WebService.Hubs;

public partial class DataHub
{
    public void AddEditStorage(string id, string key, string value)
    {
        DataService.StorageData!.AddEdit(new StorageData
            {
                Id = id == "" ? Guid.NewGuid() : Guid.Parse(id),
                Key = key,
                Value = value
            }
        );
    }

    public void RemoveStorage(string id)
    {
        DataService.StorageData!.Delete(new StorageData
            {
                Id = Guid.Parse(id),
                Key = "",
                Value = ""
            }
        );
    }

    public IEnumerable<StorageData> AllStorage()
    {
        return DataService.StorageData!.All();
    } 
}