using System.IO;
using Newtonsoft.Json;

namespace MusicPlayer.Data;

public static class SettingsWriter
{
    public static void SaveConfig(FileInfo file, Settings settings)
    {
        var json = JsonConvert.SerializeObject(settings);
        File.WriteAllText(file.FullName, json);
    }

    public static Settings DummyConfig()
    {
        return new Settings
        {
            AppName = "Lorem",
            UserAgent = "Lorem",
            X = 0,
            Y = 0,
            Width = 1920,
            Height = 1080,
            WindowLess = true,
            AlwaysOnTop = true,
            ShowDevTools = false,
            FilesToInject = new[]
            {
                "file1.js",
                "file2.js",
            }
        };
    }
}