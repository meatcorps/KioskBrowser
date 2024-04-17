using KioskBrowser.DataService.Utilities;

namespace KioskBrowser.ExternalWebService.Services;

public class CodeService
{
    private readonly (string code, DateTime date)[] _codes = GetValidCodes();

    private static (string code, DateTime date)[] GetValidCodes()
    {
        return File.ReadAllText(FileUtilities.GetExecutingDirectory("codes.txt"))
            .Replace("\r", "")
            .Split("\n")
            .Select(
                x =>
                {
                    var data = x.Split("|");
                    return (code: data[0], date: DateTime.Parse(data[1]));
                }).ToArray();
    }

    public bool IsValidCode(string code)
    {
        if (code.Length != 4)
            return false;
        
        return _codes.Any(x => x.date > DateTime.Now && code.PadLeft(4).Equals(x.code));
    }
    
    public bool IsAdmin(string code)
    {
        if (code.Length <= 4)
            return false;
        
        return _codes.Any(x => x.date > DateTime.Now && code.Equals(x.code));
    }
}