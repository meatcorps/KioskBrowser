// ReSharper disable UnassignedField.Global

namespace KioskBrowser.Core.Data;

[Serializable]
public class Settings
{
    public string? AppName;
    public string? WebUrl;
    public string? UserAgent;
    public int X;
    public int Y;
    public int Width;
    public int Height;
    public bool WindowLess;
    public bool AlwaysOnTop;
    public bool ShowDevTools;
    public string[]? FilesToInject;
    public bool? CacheEnabled;
    public string? CacheDirectory;
}