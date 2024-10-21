using System.IO.Pipes;

namespace KioskBrowser.Core.Service;

public class SingleInstanceService : IDisposable
{
    public event Action? OnAnotherInstanceDetected;
    public event Action? OnThisNeedToShutdown;
    private readonly string _pipeName;
    private bool _run = true;
    
    public SingleInstanceService(string applicationName)
    {
        _pipeName = applicationName;
        
    }
    
    public void Start()
    {
        _ = Task.Run(StartPipeServer);
    }
    
    private bool IsAnotherInstanceRunning()
    {
        try
        {
            using (var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out))
            {
                client.Connect(100); // Try to connect to the pipe within 100ms
                return client.IsConnected;
            }
        }
        catch
        {
            return false; // Pipe does not exist, so no other instance is running
        }
    }


    private void NotifyExistingInstanceToClose()
    {
        try
        {
            using (var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out))
            {
                client.ConnectAsync(100).Wait();
                if (client.IsConnected)
                    using (var writer = new StreamWriter(client))
                    {
                        writer.Write("exit");
                        writer.Flush();
                    }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to notify existing instance: " + ex.Message);
        }
    }

    
    private void StartPipeServer()
    {
        if (IsAnotherInstanceRunning())
        {
            OnAnotherInstanceDetected?.Invoke();
            NotifyExistingInstanceToClose();
            Task.Delay(100).Wait();
        }
        Console.WriteLine("Running server...");
        using (var server = new NamedPipeServerStream(_pipeName, PipeDirection.In))
        {
            while (_run)
            {
                // Wait for a client to connect
                server.WaitForConnection();
                // Disconnect and wait for the next connection
                server.Disconnect();
                OnThisNeedToShutdown?.Invoke();
            }
        }
    }

    public void Dispose()
    {
        _run = false;
    }
}