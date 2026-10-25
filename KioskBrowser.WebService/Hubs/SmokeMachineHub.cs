namespace KioskBrowser.WebService.Hubs;

public partial class DataHub
{
    public void SetSmokeMachineData(bool on, string interval)
    {
        Console.WriteLine($"Setting smoke machine data: on={on}, interval={interval}");
        _ = MqttClient.Publish("garage/smokemachine/on", on ? "1" : "0");
        _ = MqttClient.Publish("garage/smokemachine/interval", interval);
    }
}