using System.Reflection;

namespace KioskBrowser.DataService.Utilities;

public static class FileUtilities
{
    public static DirectoryInfo? GetExecutingDirectory()
    {
        var uriString = Assembly.GetEntryAssembly()!.GetName().CodeBase;
        if (uriString == null) return null;
        var location = new Uri(uriString);
        return new FileInfo(location.AbsolutePath).Directory;
    }
    
    public static FileInfo? GetExecutingFile(string name)
    {
        return new FileInfo(GetExecutingDirectory() + Path.DirectorySeparatorChar.ToString() + name);
    }
}