using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using KioskBrowser.Core.Service;
using KioskBrowser.WebService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace KioskBrowser.WebService.Bind;

public sealed class SmokeMachineBindService: IDisposable
{
    private readonly IHubContext<DataHub> _dataHubContext;
    private readonly Subject<Unit> _updateSubject = new Subject<Unit>();
    private readonly CancellationDisposable _updateSubjectDisposable = new();
    
    private bool _on;
    private int _interval;
    private bool _ready1;
    private bool _ready2;
    private int _countdown;
    
    public SmokeMachineBindService(IHubContext<DataHub> dataHubContext, KioskMqttClient mqttClient)
    {
        _dataHubContext = dataHubContext;
        _ = SubscribeToMqttTopics(mqttClient);
        _updateSubject
            .Sample(TimeSpan.FromSeconds(0.5d))
            .Subscribe(Push, _updateSubjectDisposable.Token);
    }
    
    private async Task SubscribeToMqttTopics(KioskMqttClient mqttClient)
    {
        (await mqttClient.SubscribeToTopic("garage/smokemachine/on")).Subscribe(message =>
        {
            _on = message == "1";
            _updateSubject.OnNext(Unit.Default);
        }, _updateSubjectDisposable.Token);
        (await mqttClient.SubscribeToTopic("garage/smokemachine/interval")).Subscribe(message =>
        {
            if (!int.TryParse(message, out var interval))
                return;
            _interval = interval;
            _updateSubject.OnNext(Unit.Default);
        }, _updateSubjectDisposable.Token);
        (await mqttClient.SubscribeToTopic("garage/smokemachine/ready1")).Subscribe(message =>
        {
            _ready1 = message == "1";
            _updateSubject.OnNext(Unit.Default);
        }, _updateSubjectDisposable.Token);
        (await mqttClient.SubscribeToTopic("garage/smokemachine/ready2")).Subscribe(message =>
        {
            _ready2 = message == "1";
            _updateSubject.OnNext(Unit.Default);
        }, _updateSubjectDisposable.Token);
        (await mqttClient.SubscribeToTopic("garage/smokemachine/currentTime")).Subscribe(message =>
        {
            if (!int.TryParse(message, out var countdown))
                return;
            _countdown = _interval - countdown;
            _updateSubject.OnNext(Unit.Default);
        }, _updateSubjectDisposable.Token);
    }

    private void Push(Unit __)
    {
        _ = PublishToDataHub();
    }
    
    private async Task PublishToDataHub()
    {
        await _dataHubContext.Clients.All.SendAsync("smokeMachineUpdate", _on, _interval, _ready1, _ready2, _countdown);
    }

    public void Dispose()
    {
        if (_updateSubjectDisposable.IsDisposed)
            return;
        _updateSubject.Dispose();
        _updateSubjectDisposable.Dispose();
    }
}