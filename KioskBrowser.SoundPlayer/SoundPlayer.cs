namespace KioskBrowser.SoundPlayer;
using LibVLCSharp.Shared;

public class SoundPlayer : IDisposable
{
    private readonly LibVLC _libVLC;
    private readonly MediaPlayer _mediaPlayer;
    private Media? _media;
    
    public event Action? OnPLayBackEnded;
    public TimeSpan TotalTime { get; private set; } = TimeSpan.Zero;
    
    public TimeSpan CurrentTime {
        get
        {
            if (_media == null)
                return TimeSpan.Zero;
            
            return TotalTime * _mediaPlayer.Position;
        }
    }

    public bool IsPlaying
    {
        get
        {
            if (_media == null)
                return false;
            
            return _mediaPlayer.IsPlaying;
        }
    }

    public SoundPlayer()
    {
        Core.Initialize();
        
        _libVLC = new LibVLC();
        _mediaPlayer = new MediaPlayer(_libVLC);
        _mediaPlayer.EndReached += (_, _) => OnPLayBackEnded?.Invoke();
    }

    public async Task OpenFile(string path, bool play)
    {
        _media = new Media(_libVLC, path, FromType.FromPath);
        _mediaPlayer.Media = _media;
        
        if (play)
            await Play();
    }

    public async Task Play()
    {
        if (_media == null)
            return;
        
        _mediaPlayer.Play();

        var tries = 0;
        while (!_mediaPlayer.IsPlaying && tries++ < 20)
        {
            await Task.Delay(10);
        }
        
        TotalTime = TimeSpan.FromMilliseconds(_media.Duration);
        
    }

    public void Stop()
    {
        if (_media == null)
            return;
        
        _mediaPlayer.Stop();
    }
    
    public void Pause()
    {
        if (_media == null)
            return;
        
        _mediaPlayer.Pause();
    }
    
    public void Eject()
    {
        _media?.Dispose();
        _mediaPlayer.Media = null;
        _media = null;
    }
    
    public void Seek(TimeSpan position, bool relative = false)
    {
        if (_media == null)
            return;
        
        if (relative)
            position = CurrentTime + position;
        
        var normal = (float)(position.TotalMilliseconds / TotalTime.TotalMilliseconds);
        
        normal = MathF.Min(1, MathF.Max(0, normal));
        _mediaPlayer.Position = normal;
    }
    
    public void Dispose()
    {
        _libVLC.Dispose();
        _mediaPlayer.Dispose();
        _media?.Dispose();
    }
}