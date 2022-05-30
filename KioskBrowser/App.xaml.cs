using System;
using System.IO;
using System.Windows;
using KioskBrowser.Data;

namespace KioskBrowser;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        SettingsWriter.SaveConfig(new FileInfo("template.json"),SettingsWriter.DummyConfig());
        
        if (GetSettings(e, out var settings)) return;

        var wnd = new MainWindow(settings!);
        SetWindowProperties(wnd, settings);
        wnd.Show();
    }

    private static void SetWindowProperties(MainWindow wnd, Settings? settings)
    {
        wnd.Left = settings!.X;
        wnd.Top = settings.Y;
        wnd.Width = settings.Width;
        wnd.Height = settings.Height;
        wnd.Topmost = settings.AlwaysOnTop;
        if (settings.WindowLess)
        {
            wnd.WindowStyle = WindowStyle.None;
            wnd.ResizeMode = ResizeMode.NoResize;
        }
    }

    private static bool GetSettings(StartupEventArgs e, out Settings? settings)
    {
        var settingsFile = new FileInfo(e.Args[0]);
        if (!settingsFile.Exists)
        {
            Console.WriteLine("ERROR unable to find file!");
            settings = null;
            return true;
        }

        settings = SettingsLoader.ReadConfig(settingsFile);
        return false;
    }
}