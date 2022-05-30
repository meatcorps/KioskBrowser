// ReSharper disable UnassignedField.Global

using System;

namespace MusicPlayer.Data;

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
}