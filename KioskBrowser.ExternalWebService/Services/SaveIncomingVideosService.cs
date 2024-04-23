using System.Reactive.Disposables;
using System.Reactive.Linq;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Utilities;

namespace KioskBrowser.ExternalWebService.Services;

public sealed class SaveIncomingVideosService : IDisposable
{
    private readonly CancellationDisposable _cancellationDisposable = new();
    
    public SaveIncomingVideosService(TransferService transferService)
    {
        transferService.NewCompletedTransfer.Where(x => x.Type.Contains("video"))
            .Subscribe(IncomingVideo, _cancellationDisposable.Token);
    }

    private void IncomingVideo(TransferData transferData)
    {
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory("accepted"));
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory(Path.Combine("accepted", transferData.Code + "_video")));
        
        var dataStream = transferData.GetBytes();
        
        var sha1Hash = FileUtilities.GetSha1Hash(dataStream);
        File.WriteAllBytes(FileUtilities.GetExecutingDirectory(Path.Combine("accepted", transferData.Code + "_video", sha1Hash + "." + transferData.Type.Split("/")[1])), dataStream);
        File.WriteAllText(FileUtilities.GetExecutingDirectory(Path.Combine("accepted", transferData.Code + "_video", sha1Hash + ".txt")), transferData.MetaData);
    }

    public void Dispose()
    {
        _cancellationDisposable.Dispose();
    }
}