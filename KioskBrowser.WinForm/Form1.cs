using System.Reflection;
using CefSharp;
using KioskBrowser.Data;

namespace KioskBrowser.WinForm;

public partial class Form1 : Form
{
    protected Settings Settings;
    
    public Form1(Settings settings)
    {
        Settings = settings;
        InitializeComponent();
        InjectJsFiles();
        OpenDevToolsWhenLoggin();
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
    
}