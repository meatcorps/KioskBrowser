using System.Reactive.Linq;
using System.Reactive.Subjects;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Utilities;

namespace KioskBrowser.ExternalWebService.Services;

public class MessageService
{
    public IObservable<ExternalMessage> IncomingMessage =>
        _externalMessageSubject.AsObservable();
    
    private Subject<ExternalMessage> _externalMessageSubject = new();
    
    public void SendMessage(string code, string name, string message)
    {
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory("toVerify"));
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory(Path.Combine("toVerify", code + "_mess")));

        var hash = FileUtilities.GetSha1Hash((name + message).ToArray().Select(Convert.ToByte).ToArray());
        
        File.WriteAllText(FileUtilities.GetExecutingDirectory(Path.Combine("toVerify", code + "_mess", hash + ".txt")), name + "|||" + message);
        
        _externalMessageSubject.OnNext(new ExternalMessage
        {
            Id = Guid.NewGuid(),
            Message = message,
            Code = code,
            Name = name,
            Hash = hash
        });
    }
}