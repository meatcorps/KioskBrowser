using System.Reactive.Linq;
using KioskBrowser.Core.Data;
using Microsoft.Web.WebView2.WinForms;

namespace KioskBrowser.WebView2;

public partial class Form1 : Form
{
    
    protected Settings Settings;
    private CancellationTokenRegistration _tokenRegistration = new();
    
    public Form1(Settings settings)
    {
        Settings = settings;
        InitializeComponent();
        
        webView.CreationProperties = new CoreWebView2CreationProperties
        {
            IsInPrivateModeEnabled = !Settings.CacheEnabled
        };
        
        Init();
        UpdatePosition();
        
        Text = Settings.AppName;
        TopMost = Settings.AlwaysOnTop;
        if (Settings.WindowLess)
            this.FormBorderStyle = FormBorderStyle.None;
        
        Observable
            .Interval(TimeSpan.FromSeconds(10))
            .ObserveOn(SynchronizationContext.Current)
            .Subscribe(_ => UpdatePosition(), _tokenRegistration.Token);
    }

    public void UpdatePosition()
    {
        this.Bounds = new Rectangle(Settings.X, Settings.Y, Settings.Width, Settings.Height);
        this.ClientSize = new Size(Settings.Width, Settings.Height);
    }
    
    public async void Init()
    {
        webView.Source = new Uri(Settings.WebUrl!);

        await webView.EnsureCoreWebView2Async();
        if (Settings.ShowDevTools)
            webView.CoreWebView2.OpenDevToolsWindow();
        
        if (Settings.FilesToInject == null) return;

        webView.NavigationCompleted += async (sender, args) =>
        {
            foreach (var jsFile in Settings.FilesToInject)
                await webView.ExecuteScriptAsync(File.ReadAllText(jsFile));
        }; 
        
    }
    
}