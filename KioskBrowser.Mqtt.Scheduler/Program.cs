// See https://aka.ms/new-console-template for more information

using System.Reactive;
using System.Reactive.Linq;
using KioskBrowser.Core.Service;
using KioskBrowser.Data;

var settings = SettingsLoader<SchedulerSettings>.ReadConfig(new FileInfo("settings.json"))!;

Console.WriteLine($"Starting client on {settings.Url}");

using var client = new KioskMqttClient(settings.Url!);

await client.Connect();

var filePath = string.Join(' ', args);

var schedule = SettingsLoader<Schedule>.ReadConfig(new FileInfo(filePath))!;

schedule.Items.Sort((item, scheduleItem) => item.TimeInMS > scheduleItem.TimeInMS ? 1 : -1);

var previousTime = DateTime.Now;
foreach (var item in schedule.Items)
{
    Observable.Return(Unit.Default).Delay(TimeSpan.FromMilliseconds(item.TimeInMS))
        .Subscribe(_ =>
        {
            client?.Publish(item.Topic, item.Action);
            Console.WriteLine($"Sending message on topic '{item.Topic}': {item.Action}");
        });
    
    var time = DateTime.Now + TimeSpan.FromMilliseconds(item.TimeInMS);
    Console.WriteLine($"Scheduling message on topic '{item.Topic}': {item.Action} around {time:HH:mm:ss.fff}. Between time: {((time - previousTime).TotalMilliseconds / 1000):F1}s");
    previousTime = time;
}

var closeTime = schedule.Items.Select(x => x.TimeInMS).Max() + 1000;

Console.WriteLine($"Total event time: {new TimeSpan(closeTime)}");

var run = true;

Observable.Return(Unit.Default).Delay(TimeSpan.FromMilliseconds(closeTime))
    .Subscribe(_ => run = false);

Console.CancelKeyPress += (sender, e) =>
{
    run = false;
    Console.WriteLine($"Exiting...");
};

while (run)
{
    await Task.Delay(100);
}

Environment.Exit(0);

public class SchedulerSettings
{
    public string? Url { get; set; }
}

public class Schedule
{
    public List<ScheduleItem> Items { get; set; }
}

public class ScheduleItem
{
    public int TimeInMS { get; set; }
    public string Topic { get; set; }
    public string Action { get; set; }
}