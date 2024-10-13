// See https://aka.ms/new-console-template for more information

// Create a new MQTT client

using System.Diagnostics;
using System.Text;
using KioskBrowser.Core.Service;
using KioskBrowser.Data;
using KioskBrowser.Mqtt.ExecWatcher;
using MQTTnet;
using MQTTnet.Client;

var settings = SettingsLoader<ExecWatcherSettings>.ReadConfig(new FileInfo("settings.json"))!;

Console.WriteLine($"Starting client on {settings.Url}");

var client = new KioskMqttClient(settings.Url!);

await client.Connect();
(await client.SubscribeToTopic(settings.Topic!)).Subscribe(payload =>
{
    Process.Start("cmd.exe", $"/c {payload}");
    
    Console.WriteLine($"Received message on topic '{settings.Topic}': {payload}");
});
Console.WriteLine($"Start listening on '{settings.Topic}'. Press CTRL+C to exit.");

var run = true;

Console.CancelKeyPress += (sender, e) =>
{
    run = false;
    Console.WriteLine($"Exiting...");
};

while (run)
{
    await Task.Delay(100);
}

client.Dispose();

Environment.Exit(0);