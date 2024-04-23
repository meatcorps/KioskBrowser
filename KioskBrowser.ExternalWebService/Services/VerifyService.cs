using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Utilities;

namespace KioskBrowser.ExternalWebService.Services;

public class VerifyService
{

    public MessagePicture GetMessage(string code, string type)
    {
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory("toVerify"));
        var directory = FileUtilities.GetExecutingDirectory(Path.Combine("toVerify", code + "_" + type));
        FileUtilities.MakeDirectory(directory);
        var directoryInfo = new DirectoryInfo(directory);

        var result = directoryInfo.GetFiles().FirstOrDefault(x => x.Extension.ToLower().Equals(".txt"));

        return ConvertFileToMessageObject(type, result);
    }

    public void MarkAsDownLoaded(string code, string type, string hash)
    {
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory("accepted"));
        var file = FileUtilities.GetExecutingDirectory(Path.Combine("accepted", code + "_" + type, hash + ".txt"));
        if (!File.Exists(file))
            return;
        File.Move(file, file.Replace(".txt", ".downloaded"));
    }

    public MessagePicture[] GetAllAcceptedMessages(string code, string type, int max = 10)
    {
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory("accepted"));
        var directory = FileUtilities.GetExecutingDirectory(Path.Combine("accepted", code + "_" + type));
        FileUtilities.MakeDirectory(directory);
        var directoryInfo = new DirectoryInfo(directory);

        return directoryInfo
            .GetFiles()
            .Where(x => x.Extension.ToLower().Equals(".txt"))
            .Take(max)
            .Select(x => ConvertFileToMessageObject(type, x))
            .ToArray();
    }

    private static MessagePicture ConvertFileToMessageObject(string type, FileInfo? result)
    {
        if (result is null)
            return new MessagePicture
            {
                Type = "EMPTY"
            };

        if (type == "mess")
        {
            var getAllContent = File.ReadAllText(result.FullName).Split("|||");
            return new MessagePicture
            {
                Id = result.Name.Replace(".txt", ""),
                Data = "",
                Type = type,
                Who = getAllContent[0],
                Message = getAllContent[1]
            };
        }

        var extension = "jpg";

        var binaryFile = result.Directory!.GetFiles(result.Name.Replace(".txt", ".*")).FirstOrDefault(x => !x.Name.Contains(".txt"));
        if (binaryFile is not null)
            extension = binaryFile.Extension;
            
        var message = File.ReadAllText(result.FullName);
        var location = result.FullName.Replace(".txt", extension);
        var data = Convert.ToBase64String(File.ReadAllBytes(location));
        return new MessagePicture
        {
            Id = result.Name.Replace(".txt", ""),
            Data = data,
            Type = type,
            Who = "",
            Message = message,
            Extension = location.Substring(location.Length - 3),
        };
    }

    public bool MessageVerificationResult(string code, string type, string? hash, bool accept)
    {
        if (hash is null)
            return false;
        
        var stringAccept = accept ? "accepted" : "rejected";
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory(stringAccept));
        var directory = FileUtilities.GetExecutingDirectory(Path.Combine(stringAccept, code + "_" + type, hash));
        var original = FileUtilities.GetExecutingDirectory(Path.Combine("toVerify", code + "_" + type, hash));
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory(Path.Combine(stringAccept, code + "_" + type)));
        
        if (!File.Exists(original + ".txt"))
            return false;
        
        File.Move(original + ".txt", directory + ".txt", true);
        if (type == "pho")
        {
            if (File.Exists(original + ".jpg"))
                File.Move(original + ".jpg", directory + ".jpg", true);
            if (File.Exists(original + ".gif"))
                File.Move(original + ".gif", directory + ".gif", true);
        }

        return true;
    }
    
    public int TotalMessages(string code)
    { 
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory("toVerify"));
        var messageDirectory = FileUtilities.GetExecutingDirectory(Path.Combine("toVerify", code + "_mess"));  
        FileUtilities.MakeDirectory(messageDirectory);

        var messageDirectoryInfo = new DirectoryInfo(messageDirectory);

        return messageDirectoryInfo.GetFiles().Count(x => x.Extension.ToLower().Equals(".txt"));
    }
    
    public int TotalPictures(string code)
    {
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory("toVerify"));
        var photoDirectory = FileUtilities.GetExecutingDirectory(Path.Combine("toVerify", code + "_pho"));
        FileUtilities.MakeDirectory(photoDirectory);

        var photoDirectoryInfo = new DirectoryInfo(photoDirectory);

        return photoDirectoryInfo.GetFiles().Count(x => x.Extension.ToLower().Equals(".txt"));
    }
}