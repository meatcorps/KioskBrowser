using System.Reactive.Linq;
using System.Reflection;
using CefSharp;
using KioskBrowser.Data;

namespace KioskBrowser.WinForm;

public partial class Form1 : Form
{
    protected Settings Settings;
    private CancellationTokenRegistration _tokenRegistration = new();

    public Form1(Settings settings)
    {
        Settings = settings;
        InitializeComponent();
        StartPosition = FormStartPosition.Manual;
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        this.Text = Settings.AppName;
        this.TopMost = Settings.AlwaysOnTop;
        if (Settings.WindowLess)
            this.FormBorderStyle = FormBorderStyle.None;

        InitializeChromium();
        InjectJsFiles();
        OpenDevToolsWhenLoggin();
        UpdatePosition();
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
    
    
    private void InjectJsFiles()
    {
        if (Settings.FilesToInject == null) return;

        foreach (var jsFile in Settings.FilesToInject)
            ChromeBrowser.ExecuteScriptAsyncWhenPageLoaded(File.ReadAllText(jsFile));;
    }

    private void OpenDevToolsWhenLoggin()
    {
        ChromeBrowser.AddressChanged += (sender, args) =>
        {
            if (ChromeBrowser.Address.ToLower().Contains("accounts.google.com")) 
                ChromeBrowser.ShowDevTools();

            if (Settings.ShowDevTools)
            {
                Settings.ShowDevTools = false;
                ChromeBrowser.ShowDevTools();
            }
        };
    }
    
    private static DirectoryInfo? GetExecutingDirectory()
    {
        var uriString = Assembly.GetEntryAssembly()!.GetName().CodeBase;
        if (uriString == null) return null;
        var location = new Uri(uriString);
        return new FileInfo(location.AbsolutePath).Directory;
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _tokenRegistration.Dispose();
    }
}