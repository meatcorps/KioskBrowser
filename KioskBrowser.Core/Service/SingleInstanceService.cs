namespace KioskBrowser.Core.Service;

public class SingleInstanceService : IDisposable
{
    public event Action? OnAnotherInstanceDetected;
    public event Action? OnShutdown;
    private readonly string _applicationName;
    private Mutex _mutex;
    private EventWaitHandle _stopEvent;
    private bool _run = true;
    
    public SingleInstanceService(string applicationName, bool directStart = false)
    {
        _applicationName = applicationName;
        _mutex = new Mutex(false, applicationName);
        _stopEvent = new EventWaitHandle(false, EventResetMode.ManualReset, $"{applicationName}StopEvent");
        
        if (directStart)
            Start();
    }
    
    public void Start()
    {
        if (!_mutex.WaitOne(TimeSpan.Zero, true))
        {
            // If another instance is running, signal it to stop
            OnAnotherInstanceDetected?.Invoke();
            Console.WriteLine("Another instance is already running. Signaling it to stop...");
            _stopEvent.Set(); // Signal the first instance to stop
            Thread.Sleep(100);

            _stopEvent.Reset();
            _mutex.Dispose();
            _mutex = new Mutex(true, _applicationName);
        }

        Task.Run(MonitorStopEvent);
    }

    private void MonitorStopEvent()
    {
        while (_run)
        {
            if (_stopEvent.WaitOne(0))
            {
            
                OnShutdown?.Invoke();
                Dispose();
                break;
            }
            
            Thread.Sleep(100);
        }
    }

    public void Dispose()
    {
        _mutex.ReleaseMutex();
        _mutex.Dispose();
        _stopEvent.Dispose();
        _run = false;
    }
}