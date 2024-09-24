using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Utilities;
using Medallion;
using Newtonsoft.Json;

namespace KioskBrowser.WebService.Services;

public class MessagePictureImporter: IDisposable
{
    public string Code { get; }
    private readonly string _adminCode;
    public string Url { get; }
    private CancellationDisposable _cancellationDisposable = new();
    private HttpClient _httpClient = new ();
    private DirectoryInfo _downloadedFilesDirectory;

    private readonly List<string> _quePhotos = new ();
    private readonly List<string> _queMessages = new ();
    private readonly Queue<string> _quePhotosPrio = new ();
    private readonly Queue<string> _queMessagesPrio = new ();
    private volatile bool _isImporting = false;
    
    public MessagePictureImporter(string code, string adminCode, string url)
    {
        Code = code;
        _adminCode = adminCode;
        Url = url;
        _downloadedFilesDirectory = new DirectoryInfo(FileUtilities.GetExecutingDirectory("partymessages"));
    }
    
    public MessagePicture NextPicture()
    {
        if (!_quePhotosPrio.TryDequeue(out var target))
        {
            if (_quePhotos.Count == 0)
                QueListFillForType(_quePhotos, "PHO");
            
            target = _quePhotos[0];
            _quePhotos.RemoveAt(0);
        }

        if (string.IsNullOrEmpty(target))
            return new MessagePicture();

        return JsonConvert.DeserializeObject<MessagePicture>(
            File.ReadAllText(
                _downloadedFilesDirectory.FullName + Path.DirectorySeparatorChar + target))!;
    }

    public MessagePicture NextMessage()
    {
        if (!_queMessagesPrio.TryDequeue(out var target))
        {
            if (_queMessages.Count == 0)
                QueListFillForType(_queMessages, "MESS");
            
            target = _queMessages[0];
            _queMessages.RemoveAt(0);
        }

        if (string.IsNullOrEmpty(target))
            return new MessagePicture();

        return JsonConvert.DeserializeObject<MessagePicture>(
            File.ReadAllText(
                _downloadedFilesDirectory.FullName + Path.DirectorySeparatorChar + target))!;
    }

    public async Task Start()
    {
        if (!_downloadedFilesDirectory.Exists)
            _downloadedFilesDirectory.Create();

        FillQueueList();
        
        await DoImport();

        Observable.Interval(TimeSpan.FromSeconds(30)).Where(_ => !_isImporting).Subscribe(_ =>
            Task.Run(async () => await DoImport())
        , _cancellationDisposable.Token);
    }

    private void FillQueueList()
    {
        QueListFillForType(_quePhotos, "PHO");
        QueListFillForType(_queMessages, "MESS");
    }

    void QueListFillForType(List<string> target, string type)
    {
        target.AddRange(_downloadedFilesDirectory
            .GetFiles()
            .Select(x => x.Name)
            .Where(x => x.Contains(type))
            .Shuffled());
    }

    private async Task DoImport()
    {
        _isImporting = true;
        await DoImportOfSpecificType("pho", _quePhotosPrio);
        await DoImportOfSpecificType("mess", _queMessagesPrio);
        await DoImportOfSpecificType("video", null);
        _isImporting = false;
    }

    private async Task DoImportOfSpecificType(string type, Queue<string>? target)
    {
        while (true)
        {
            var url = $"{Url}/transfer/download/{type}/{_adminCode}/{Code}";
            // Only the first 10 fill show up here.
            var result = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                url));
            if (result.IsSuccessStatusCode)
            {
                var items = await result.Content.ReadFromJsonAsync<MessagePicture[]>();
                foreach (var item in items)
                {
                    if (type == "video")
                    {
                        await File.WriteAllBytesAsync(
                            _downloadedFilesDirectory.FullName + Path.DirectorySeparatorChar + type.ToUpper() +
                            item.Id + "." + item.Extension,
                            Convert.FromBase64String(item.Data));

                        await File.WriteAllTextAsync(
                            _downloadedFilesDirectory.FullName + Path.DirectorySeparatorChar + type.ToUpper() +
                            item.Id + ".txt",
                            item.Message);
                    }
                    else     
                        await File.WriteAllTextAsync(
                            _downloadedFilesDirectory.FullName + Path.DirectorySeparatorChar + type.ToUpper() + item.Id,
                            JsonConvert.SerializeObject(item));

                    // We're going to accept the message, so it will not show up next time. 
                    var resultack = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                        $"{Url}/transfer/ack/{type}/{_adminCode}/{Code}/{item.Id}"));
                    
                    if (!resultack.IsSuccessStatusCode)
                        Console.WriteLine($"Something is going wrong with ACK message {item.Id} ");
                    
                    target?.Enqueue(type.ToUpper() + item.Id);
                }
                
                Console.WriteLine($"Done {type}! " + items.Length);
                
                if (items.Length == 0)
                    break;
            }
            else
            {
                Console.WriteLine($"Something is going wrong with getting messages of type {type} ");
                break;
            }
            await Task.Delay(1000);
        }
    }

    public void Dispose()
    {
        _cancellationDisposable.Dispose();
        _httpClient.Dispose();
    }
}