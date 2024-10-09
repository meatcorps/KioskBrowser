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
using KioskBrowser.Data;

namespace KioskBrowser.MediaPlayer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    private readonly MediaPlayerSettings _settings;

    public MainWindow()
    {
        _settings = SettingsLoader<MediaPlayerSettings>.ReadConfig(new FileInfo("settings.json"))!;
        InitializeComponent();
        var file = new FileInfo(_settings.VideoPath!);
        mePlayer.Source = new Uri(file.FullName);
        mePlayer.IsMuted = _settings.Muted;
        mePlayer.LoadedBehavior = MediaState.Play;
        mePlayer.UnloadedBehavior = MediaState.Manual;
        Cursor = Cursors.None;
        UpdateScreen();
        Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(_ => UpdateScreen());
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

    private void MePlayer_OnMediaEnded(object sender, RoutedEventArgs e)
    {
        if (_settings.Repeat)
        {
            mePlayer.Position = new TimeSpan(0,0,1);
            mePlayer.Play();
        }
        else
            Close();
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
}