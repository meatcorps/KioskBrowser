// See https://aka.ms/new-console-template for more information

// Create a new MQTT client

using System.Diagnostics;
using System.Text;
using MQTTnet;
using MQTTnet.Client;

var factory = new MqttFactory();
var mqttClient = factory.CreateMqttClient();

// Define the MQTT client options
var options = new MqttClientOptionsBuilder()
    .WithCredentials("user", "admin")
    .WithTcpServer("app.emmastraat67.local", 1883) // Replace with your MQTT broker address
    .WithCleanSession()
    .Build();

// Define a handler for received messages
mqttClient.ApplicationMessageReceivedAsync += e =>
{
    var topic = e.ApplicationMessage.Topic;
    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

    Process.Start("cmd.exe", $"/c {payload}");
    Console.WriteLine("test");
    
    Console.WriteLine($"Received message on topic '{topic}': {payload}");
    return Task.CompletedTask;
};

// Connect to the MQTT broker
await mqttClient.ConnectAsync(options, CancellationToken.None);
Console.WriteLine("Connected to the MQTT broker.");

// Subscribe to a topic
var topicToSubscribe = "test/topic";
await mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
    .WithTopicFilter(f => f.WithTopic(topicToSubscribe))
    .Build());

Console.WriteLine($"Subscribed to topic '{topicToSubscribe}'.");

// Keep the application running
Console.WriteLine("Press any key to exit.");
Console.ReadLine();

// Disconnect the client
await mqttClient.DisconnectAsync();