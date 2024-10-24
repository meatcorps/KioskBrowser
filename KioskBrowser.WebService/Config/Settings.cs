﻿// ReSharper disable UnassignedField.Global

namespace KioskBrowser.WebService.Config;

[Serializable]
public class Settings
{
    public string[]? StartArguments;
    public string? PictureFileWatchLocation;
    public string? PictureUrlLocation;
    public string? ExternalAdminCode;
    public string? ExternalCode;
    public string? ExternalUrl;
}