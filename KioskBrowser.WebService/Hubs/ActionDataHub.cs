using KioskBrowser.DataService.Data;

namespace KioskBrowser.WebService.Hubs;

public partial class DataHub
{
    public void AddEditAction(string id, string name, string action, string target)
    {
        Console.WriteLine("Adding action: " + id + " " + name + " " + action + " " + target);
        DataService.ActionData!.AddEdit(new ActionData
            {
                Id = id == "" ? Guid.NewGuid() : Guid.Parse(id),
                Name = name,
                Action = action,
                Target = target
            }
        );
    }
    
    public void DoAction(string id)
    {
        var action = DataService.ActionData!.Get(Guid.Parse(id));
        
        if (action is null)
            return;
        
        _ = MqttClient.Publish(action.Target, action.Action);
    }

    public void RemoveAction(string id)
    {
        DataService.ActionData!.Delete(new ActionData
            {
                Id = Guid.Parse(id),
                Name = "",
                Action = "",
                Target = ""
            }
        );
    }

    public IEnumerable<ActionData> AllActions()
    {
        return DataService.ActionData!.All();
    } 
}