using System.IO;
using Newtonsoft.Json;

namespace KioskBrowser.Data;

public static class SettingsLoader<T>
{
    public static T? ReadConfig(FileInfo file)
    {
        var stream = File.ReadAllText(file.FullName);
        
        var settings = JsonConvert.DeserializeObject<T>(stream);

        if (settings is null)
            return default;
        
        if (settings is Settings fixPathSettings)
            FixPathForSettings(file, fixPathSettings);
        
        return settings;
    }

    private static void FixPathForSettings(FileInfo file, Settings settings)
    {
        for (var i = 0; i < settings?.FilesToInject?.Length; i++)
        {
            settings!.FilesToInject![i] = file.Directory?.FullName + Path.DirectorySeparatorChar + settings.FilesToInject[i];
        }

        if (settings!.CacheDirectory is null)
        {
            settings.CacheDirectory = "webcache";
        }
    }
}