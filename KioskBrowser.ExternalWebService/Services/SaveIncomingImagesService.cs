using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Security.Cryptography;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Utilities;
using SkiaSharp;

namespace KioskBrowser.ExternalWebService.Services;

public sealed class SaveIncomingImagesService: IDisposable
{
    public IObservable<TransferData> ReadyToVerify =>
        _readyToVerifySubject.AsObservable();
    
    private readonly Subject<TransferData> _readyToVerifySubject = new();
    
    private readonly CancellationDisposable _cancellationDisposable = new();

    public SaveIncomingImagesService(TransferService transferService)
    {
        transferService.NewCompletedTransfer.Where(x => x.Name.Contains(".jpg") || x.Name.Contains(".jpeg")).Subscribe(IncomingImage, _cancellationDisposable.Token);
    }

    private void IncomingImage(TransferData transferData)
    {
        var dataStream = transferData.GetBytes();

        using var temp = SKBitmap.Decode(dataStream);
        using var bitmap = ResizeImage(temp);

        using var data = bitmap.Encode(SKEncodedImageFormat.Jpeg, 75);
        
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory("toVerify"));
        FileUtilities.MakeDirectory(FileUtilities.GetExecutingDirectory(Path.Combine("toVerify", transferData.Code + "_pho")));

        var sha1Hash = FileUtilities.GetSha1Hash(data.ToArray());

        transferData.Hash = sha1Hash;
        transferData.Chunks.Dispose();
        
        using var fileWriteStream = File.Create(FileUtilities.GetExecutingDirectory(Path.Combine("toVerify", transferData.Code + "_pho", sha1Hash + ".jpg")));
        data.SaveTo(fileWriteStream);
        File.WriteAllText(FileUtilities.GetExecutingDirectory(Path.Combine("toVerify", transferData.Code + "_pho", sha1Hash + ".txt")), transferData.MetaData);
        _readyToVerifySubject.OnNext(transferData);
    }
    
    private static SKBitmap ResizeImage(SKBitmap bitmap)
    {
        if (bitmap.Width > 1280)
        {
            var ratio = (float)bitmap.Height / bitmap.Width;
            var newHeight = (int)(1280 * ratio);
            using var resizedBitmap = bitmap.Resize(new SKSizeI(1280, newHeight), SKFilterQuality.High);

            resizedBitmap.CopyTo(bitmap);
        }
        
        if (bitmap.Height > 1024)
        {
            var ratio = (float)bitmap.Width / bitmap.Height;
            var newWidth = (int)(1024 * ratio);
            using var resizedBitmap = bitmap.Resize(new SKSizeI(newWidth, 1024), SKFilterQuality.High);
            
            resizedBitmap.CopyTo(bitmap);
        }

        return bitmap;
    }

    public void Dispose()
    {
        _cancellationDisposable.Dispose();
    }
}