using KioskBrowser.ExternalWebService.Services;
using Microsoft.AspNetCore.SignalR;

namespace KioskBrowser.ExternalWebService.Hubs;

public class TransferHub : Hub
{
    private readonly TransferService _transferService;
    private readonly CodeService _codeService;

    public TransferHub(TransferService transferService, CodeService codeService)
    {
        _transferService = transferService;
        _codeService = codeService;
    }
    
    public string TransferRequest(string code, string name, int filesize, string metaData, string type)
    {
        if (!_codeService.IsValidCode(code))
            return "INVALID";
        
        return _transferService.NewBase64Transfer(code, name, filesize, metaData, type).ToString();
    }
    
    public int ChunkSize()
    {
        return _transferService.ChunkSize;
    }
    
    public int TotalChunks(string id)
    {
        return _transferService.TotalChunks(Guid.Parse(id));
    }
    
    public async Task<int> SendData(string id, int chunkNr, string base64data)
    {
        return _transferService.SetData(Guid.Parse(id), chunkNr, base64data);
    }
}

