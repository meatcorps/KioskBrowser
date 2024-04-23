using System.Text;
using KioskBrowser.ExternalWebService.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KioskBrowser.ExternalWebService.Controllers;

[ApiController]
[Route("[controller]")]
public class TransferController : ControllerBase
{
    private readonly VerifyService _verifyService;
    private readonly TransferService _service;
    private readonly CodeService _codeService;
    private readonly ILogger<TransferController> _logger;

    public TransferController(VerifyService verifyService, TransferService service, CodeService codeService, ILogger<TransferController> logger)
    {
        _verifyService = verifyService;
        _service = service;
        _codeService = codeService;
        _logger = logger;
    }

    [HttpPost]
    [Route("upload/{guid}/{chunk}")]
    public async Task<int> Upload(string guid, int chunk)
    {
        var rawContent = string.Empty;
        using (var reader = new StreamReader(Request.Body,
                   encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
        {
            rawContent = await reader.ReadToEndAsync();
        }

        return _service.SetData(Guid.Parse(guid), chunk, rawContent);
    }

    [HttpGet]
    [Route("accept/{type}/{adminCode}/{code}")]
    public string Accept(string type, string adminCode, string code)
    {
        if (!_codeService.IsValidCode(code))
            return "NOPE";
        if (!_codeService.IsAdmin(adminCode))
            return "NOPE";
        if (type is not ("pho" or "mess"))
            return "NOPE";
        
        var message = _verifyService.GetMessage(code, type);
        
        return JsonConvert.SerializeObject(_verifyService.MessageVerificationResult(code, type, message.Id, true) ? "OK" : "GHOST");
    }

    [HttpGet]
    [Route("decline/{type}/{adminCode}/{code}")]
    public string Decline(string type, string adminCode, string code)
    {
        if (!_codeService.IsValidCode(code))
            return "NOPE";
        if (!_codeService.IsAdmin(adminCode))
            return "NOPE";
        if (type is not ("pho" or "mess"))
            return "NOPE";

        var message = _verifyService.GetMessage(code, type);
        
        return JsonConvert.SerializeObject(_verifyService.MessageVerificationResult(code, type, message.Id, false) ? "OK" : "GHOST");
    }
    
    
    [HttpGet]
    [Route("download/{type}/{adminCode}/{code}")]
    public string Download(string type, string adminCode, string code)
    {
        if (!_codeService.IsValidCode(code))
            return "NOPE";
        if (!_codeService.IsAdmin(adminCode))
            return "NOPE";
        if (type is not ("pho" or "mess" or "video"))
            return "NOPE";

        var message = _verifyService.GetAllAcceptedMessages(code, type);
        
        return JsonConvert.SerializeObject(message);
    }
    
    [HttpGet]
    [Route("ack/{type}/{adminCode}/{code}/{hash}")]
    public string Ack(string type, string adminCode, string code, string hash)
    {
        if (!_codeService.IsValidCode(code))
            return "NOPE";
        if (!_codeService.IsAdmin(adminCode))
            return "NOPE";
        if (type is not ("pho" or "mess" or "video"))
            return "NOPE";

        _verifyService.MarkAsDownLoaded(code, type, hash);
        
        return JsonConvert.SerializeObject(true);
    }

    [HttpGet]
    [Route("verify/{type}/{adminCode}/{code}")]
    public string VerifyMessage(string type, string adminCode, string code)
    {
        if (!_codeService.IsValidCode(code))
            return "NOPE";
        if (!_codeService.IsAdmin(adminCode))
            return "NOPE";

        var dictionary = new Dictionary<string, object>();

        switch (type)
        {
            case "mess":
                dictionary["total"] = _verifyService.TotalMessages(code);
                dictionary["object"] = _verifyService.GetMessage(code, type);
                break;
            case "pho":
                dictionary["total"] = _verifyService.TotalPictures(code);
                dictionary["object"] = _verifyService.GetMessage(code, type);
                break;
        }

        return JsonConvert.SerializeObject(dictionary);
    }
}