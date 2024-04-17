using Microsoft.AspNetCore.SignalR;

namespace KioskBrowser.ExternalWebService.Hubs;

public class PingHub : Hub
{
    public string Ping()
    {
        return "Pong";
    }
    
    public string ReturnMessage(string message)
    {
        return $"Got message: {message}";
    }
}