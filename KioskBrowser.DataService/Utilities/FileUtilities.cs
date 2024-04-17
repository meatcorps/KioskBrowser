using System.Reflection;
using System.Security.Cryptography;

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
    
    public static string GetExecutingDirectory(string path)
    {
        var startDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        return startDirectory + Path.DirectorySeparatorChar + path;
    }
    
    public static void MakeDirectory(string path)
    {
        if (Directory.Exists(path))
            return;

        Directory.CreateDirectory(path);
    }
    
    public static string GetSha1Hash(byte[] data)
    {
        using var sha1Hash = SHA1.Create();
        var hash = sha1Hash.ComputeHash(data);
        return BitConverter.ToString(hash).Replace("-", String.Empty);
    }
}