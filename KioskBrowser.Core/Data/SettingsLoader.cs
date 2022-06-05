using System.IO;
using Newtonsoft.Json;

namespace KioskBrowser.Data;

public static class SettingsLoader
{
    public static Settings? ReadConfig(FileInfo file)
    {
        var stream = File.ReadAllText(file.FullName);
        
        var settings = JsonConvert.DeserializeObject<Settings>(stream);

        if (settings is null)
            return null;
        
        FixPathForSettings(file, settings);
        
        return settings;
    }

    private static void FixPathForSettings(FileInfo file, Settings settings)
    {
        for (var i = 0; i < settings?.FilesToInject?.Length; i++)
        {
            settings!.FilesToInject![i] = file.Directory?.FullName + Path.DirectorySeparatorChar + settings.FilesToInject[i];
        }
    }
}