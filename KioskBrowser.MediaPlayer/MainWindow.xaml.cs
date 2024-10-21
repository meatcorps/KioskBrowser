using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using KioskBrowser.Core.Service;
using KioskBrowser.Data;
using LibVLCSharp.Shared;
using Vlc.DotNet.Wpf;
using Path = System.IO.Path;

namespace KioskBrowser.MediaPlayer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    private readonly MediaPlayerSettings _settings;
    private readonly KioskMqttClient _client;
    private bool _originalVideo = true;
    private bool _repeatFromTopic = false;
    private bool _returnToOriginalFromTopic = false;
    private string _lastLocation = "";

    
    public MainWindow()
    {
        _settings = SettingsLoader<MediaPlayerSettings>.ReadConfig(new FileInfo("settings.json"))!;
        InitializeComponent();
        var file = new FileInfo(_settings.VideoPath!);
        
        Cursor = Cursors.None;
        UpdateScreen();
        Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(_ => UpdateScreen());
        Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(_ => CheckPosition());

        _client = new KioskMqttClient(_settings.Url);
        
        InitializeBroker();
        InitializeVlc();
        
        mePlayer.SourceProvider.MediaPlayer.Play(new Uri(file.FullName));
    }

    private void CheckPosition()
    {
        if (mePlayer.SourceProvider.MediaPlayer == null)
            return;
        
        var totalTime = mePlayer.SourceProvider.MediaPlayer.Length;
        var timeLeft = totalTime - (mePlayer.SourceProvider.MediaPlayer.Position * totalTime);
        
        if (timeLeft < 200 && mePlayer.SourceProvider.MediaPlayer.IsSeekable)
        {
            if ((_settings.Repeat && _originalVideo) || (!_originalVideo && _repeatFromTopic))
            {
                mePlayer.SourceProvider.MediaPlayer.Position = 100 / (float) totalTime;
            }
        }

        if (!mePlayer.SourceProvider.MediaPlayer.IsPlaying() && !_returnToOriginalFromTopic)
        {
            mePlayer.SourceProvider.MediaPlayer.Play();
        }

    }

    private void InitializeVlc()
    {
        var directory = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        var vlcLibDirectory = new DirectoryInfo(Path.Combine(directory.Directory!.FullName, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

        var options = new string[]
        {
            // VLC options can be given here. Please refer to the VLC command line documentation.
        };
        
        mePlayer.SourceProvider.CreatePlayer(vlcLibDirectory, options);
        mePlayer.SourceProvider.MediaPlayer.EndReached += (_, _) => MePlayer_OnMediaEnded();
        mePlayer.SourceProvider.MediaPlayer.Audio.IsMute = _settings.Muted;
    }

    private void InitializeBroker()
    {
        if (!string.IsNullOrEmpty(_settings.Topic))
            _ = Task.Run(async () =>
            {
                await _client.Connect();
                (await _client.SubscribeToTopic(_settings.Topic)).Subscribe(data =>
                {
                    _repeatFromTopic = false;
                    _returnToOriginalFromTopic = false;
                    
                    
                    if (data.Length > 6 && data.Substring(0, 6) == "repeat")
                    {
                        _repeatFromTopic = true;
                        data = data.Substring(6).Trim();
                    }
                    
                    if (data.Length > 4 && data.Substring(0, 4) == "once")
                    {
                        _returnToOriginalFromTopic = true;
                        data = data.Substring(4).Trim();
                    }
                    
                    _originalVideo = !File.Exists(data);
                    var file = new FileInfo(_settings.VideoPath!);
                    
                    if (!_originalVideo)
                        file = new FileInfo(data);
                    
                    Play(file.FullName);
                    Debug.WriteLine("Start: " + file.FullName);
                });
            });
    }

    private void UpdateScreen()
    {
        Dispatcher.Invoke(() =>
        {
            Title = _settings.AppName;
            Width = _settings.Width;
            Height = _settings.Height;
            Top = _settings.Y;
            Left = _settings.X;
            mePlayer.Width = Width;
            mePlayer.Height = Height;
        });
    }

    private void MePlayer_OnMediaEnded()
    {
        if ((!_originalVideo) || (_originalVideo && _settings.Repeat))
        {
            var path = string.Empty;
            if (_returnToOriginalFromTopic)
            {
                _returnToOriginalFromTopic = false;
                _originalVideo = true;
                path = _settings.VideoPath!;
            }

            if (!_originalVideo && !_repeatFromTopic)
            {
                _originalVideo = false;
                path = _settings.VideoPath!;
            }

            InitializeVlc();
            Play(path);
            return;
        }
        
        if (!_originalVideo || _settings.Repeat)
            return;
        
        Dispatcher.Invoke(() => Close());
    }

    private void Play(string location = "")
    {
        if (string.IsNullOrEmpty(location))
            location = _lastLocation;

        if (string.IsNullOrEmpty(location))
            location = _settings.VideoPath!;
        
        Dispatcher.Invoke(() => mePlayer.SourceProvider.MediaPlayer.Play(new Uri(new FileInfo(location).FullName)));
    }
}

public class MediaPlayerSettings()
{
    public string? AppName;
    public string? VideoPath;
    public int X;
    public int Y;
    public int Width;
    public int Height;
    public bool Repeat;
    public bool Muted;
    public string Topic { get; set; }
    public string Url { get; set; }
}