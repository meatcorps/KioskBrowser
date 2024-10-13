namespace KioskBrowser.DataService.Data;

[Serializable]
public class CollectionData
{
    public List<GroupData> Groups = new ();
    public List<ProductData> Products = new ();
    public List<MessageData> Messages = new ();
    public List<StorageData> Storage = new ();
    public List<ActionData> Actions = new ();
}