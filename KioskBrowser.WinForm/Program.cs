using KioskBrowser.Data;

namespace KioskBrowser.WinForm;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var args = Environment.GetCommandLineArgs();

        if (!GetSettings(args, out var settings) || settings is null) return;
        
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1(settings));
    }
    
    private static bool GetSettings(string[] args, out Settings? settings)
    {
        if (args.Length < 2)
        {
            settings = new Settings();
            return false;
        }

        var settingsFile = new FileInfo(args[1]);
        if (!settingsFile.Exists)
        {
            Console.WriteLine("ERROR unable to find file!");
            settings = null;
            return false;
        }

        settings = SettingsLoader<Settings>.ReadConfig(settingsFile);
        return true;
    }
}