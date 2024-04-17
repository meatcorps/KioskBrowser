// ReSharper disable UnassignedField.Global

namespace KioskBrowser.ExternalWebService.Config;

[Serializable]
public class Settings
{
    public string? VapidPushPrivate;
    public string? VapidPushPublic;
    public int? ChunkSize;
}