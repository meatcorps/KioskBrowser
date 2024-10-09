namespace KioskBrowser.Core.Service;

public class SingleInstanceService : IDisposable
{
    public event Action? OnAnotherInstanceDetected;
    public event Action? OnThisNeedToShutdown;
    private readonly string _applicationName;
    private EventWaitHandle _stopEvent;
    
    private bool _run = true;
    
    public SingleInstanceService(string applicationName)
    {
        _applicationName = applicationName;
        
        _stopEvent = new EventWaitHandle(false, EventResetMode.ManualReset, $"{applicationName}StopEvent");
    }
    
    public void Start()
    {
        _ = Task.Run(MonitorStopEvent);
    }

    private async Task MonitorStopEvent()
    {
        var mutex = new Mutex(false, _applicationName);
        
        if (!mutex.WaitOne(TimeSpan.Zero, true))
        {
            // If another instance is running, signal it to stop
            _ = Task.Run(() => OnAnotherInstanceDetected?.Invoke());
            _stopEvent.Set(); // Signal the first instance to stop
            await Task.Delay(100);

            _stopEvent.Reset();
            mutex.Dispose();
            mutex = new Mutex(true, _applicationName);
        }
        
        while (_run)
        {
            if (_stopEvent.WaitOne(0))
            {
            
                Dispose();
                break;
            }
            
            await Task.Delay(100);
        }
        
        mutex.ReleaseMutex();
        mutex.Dispose();
        _ = Task.Run(() => OnThisNeedToShutdown?.Invoke());
    }

    public void Dispose()
    {
        _stopEvent.Dispose();
        _run = false;
    }
}