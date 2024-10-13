using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using MQTTnet;
using MQTTnet.Client;

namespace KioskBrowser.Core.Service;

public sealed class KioskMqttClient : IDisposable
{
    private readonly string _url;
    private readonly IMqttClient _client;
    
    private Subject<Tuple<string, string>> _messageReceived = new();

    public KioskMqttClient(string url)
    {
        _url = url;
        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();
    }

    public async Task Connect()
    {
        var options = new MqttClientOptionsBuilder()
            .WithCredentials("user", "admin")
            .WithTcpServer(_url, 1883) // Replace with your MQTT broker address
            .WithCleanSession()
            .Build();

        _client.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            _messageReceived.OnNext(new Tuple<string, string> (item1: topic, item2: payload));

            return Task.CompletedTask;
        };

        await _client.ConnectAsync(options, CancellationToken.None);
    }

    public async Task<IObservable<string>> SubscribeToTopic(string topic)
    {
        var returnObservable = _messageReceived.Where(x => x.Item1.Equals(topic))
            .Select(x => x.Item2)
            .AsObservable();
        
        await _client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic(topic))
            .Build());

        return returnObservable;
    }
    
    public async Task Publish(string topic, string payload)
    {
        await _client.PublishAsync(new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build());
    }

    public void Dispose()
    {
        _client.DisconnectAsync().Wait();
        _messageReceived.Dispose();
        _client.Dispose();
    }
}