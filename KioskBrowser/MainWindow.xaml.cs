using System;
using System.IO;
using System.Reflection;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using KioskBrowser.Data;

namespace KioskBrowser;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Settings _settings;

    public MainWindow(Settings settings)
    {
        _settings = settings;

        PreInitializeCef();

        InitializeComponent();

        OpenDevToolsWhenLoggin();

        InjectJsFiles();
    }

    public string? WebUrl => _settings.WebUrl;
    public string? LabelName => _settings.AppName;

    private void InjectJsFiles()
    {
        if (_settings.FilesToInject == null) return;

        foreach (var jsFile in _settings.FilesToInject)
            Browser.ExecuteScriptAsyncWhenPageLoaded(File.ReadAllText(jsFile));
    }

    private void OpenDevToolsWhenLoggin()
    {
        Browser.AddressChanged += (sender, args) =>
        {
            if (Browser.Address.ToLower().Contains("accounts.google.com")) Browser.ShowDevTools();

            if (_settings.ShowDevTools)
            {
                _settings.ShowDevTools = false;
                Browser.ShowDevTools();
            }
        };
    }

    private void PreInitializeCef()
    {
        var cefSettings = new CefSettings();
        cefSettings.CachePath = GetExecutingDirectory()!.FullName + "\\webcache\\";
        if (_settings.UserAgent is not null)
            cefSettings.UserAgent = _settings.UserAgent; 
        if (Cef.IsInitialized == false)
            Cef.Initialize(cefSettings);
    }

    private static DirectoryInfo? GetExecutingDirectory()
    {
        var uriString = Assembly.GetEntryAssembly()!.GetName().CodeBase;
        if (uriString == null) return null;
        var location = new Uri(uriString);
        return new FileInfo(location.AbsolutePath).Directory;
    }
}