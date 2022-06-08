using System.Text.RegularExpressions;
using KioskBrowser.DataService.Data;

namespace KioskBrowser.WebService.Hubs;

public partial class DataHub
{
    public void AddEditProduct(string id, string title, string group, int sortIndex, int totalItems)
    {
        var realGroupItem = DataService.GroupData!.Get(Guid.Parse(group));

        if (realGroupItem is null)
            return;
        
        DataService.ProductData!.AddEdit(new ProductData
            {
                Id = id == "" ? Guid.NewGuid() : Guid.Parse(id),
                Name = title,
                Group = realGroupItem.Id,
                SortIndex = sortIndex,
                TotalItems = totalItems
            }
        );
    }

    public void RemoveProduct(string id)
    {
        DataService.ProductData!.Delete(new ProductData
            {
                Id = Guid.Parse(id),
                Name = "",
                Group = Guid.Empty,
                SortIndex = 0,
                TotalItems = 0
            }
        );
    }

    public IEnumerable<ProductData> AllProducts()
    {
        return DataService.ProductData!.All();
    } 
}