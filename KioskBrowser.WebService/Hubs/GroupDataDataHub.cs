using KioskBrowser.DataService.Data;

namespace KioskBrowser.WebService.Hubs;

public partial class DataHub
{
    public void AddEditGroup(string id, string groupName, int sortIndex)
    {
        DataService.GroupData!.AddEdit(new GroupData
            {
                Id = id == "" ? Guid.NewGuid() : Guid.Parse(id),
                Name = groupName,
                SortIndex = sortIndex
            }
        );
    }

    public void RemoveGroup(string id)
    {
        DataService.GroupData!.Delete(new GroupData
            {
                Id = Guid.Parse(id),
                Name = "",
                SortIndex = 0
            }
        );
    }

    public IEnumerable<GroupData> AllGroups()
    {
        return DataService.GroupData!.All();
    } 
}