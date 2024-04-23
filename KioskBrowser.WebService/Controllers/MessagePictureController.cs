using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Services;
using KioskBrowser.WebService.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRCoder;

namespace KioskBrowser.WebService.Controllers;

[ApiController]
public class MessagePictureController : ControllerBase
{
    private readonly MessagePictureImporter _messagePictureImporter;
    private readonly ILogger<MessagePictureController> _logger;

    public MessagePictureController(MessagePictureImporter messagePictureImporter, ILogger<MessagePictureController> logger)
    {
        _messagePictureImporter = messagePictureImporter;
        _logger = logger;
    }

    [HttpGet]
    [Route("currentcode")]
    public string CurrentCode()
    {
        return JsonConvert.SerializeObject(_messagePictureImporter.Code);
    }

    [HttpGet]
    [Route("qrcode")]
    public IActionResult QrCode()
    {
        var qrGenerator = new QRCodeGenerator();
        var data = qrGenerator.CreateQrCode(_messagePictureImporter.Url, QRCodeGenerator.ECCLevel.Q);
        var code = new PngByteQRCode(data);
        
        return File(code.GetGraphic(20), "image/png");
    }

    [HttpGet]
    [Route("nextmessage")]
    public MessagePicture NextMessage()
    {
        return _messagePictureImporter.NextMessage();
    }

    [HttpGet]
    [Route("nextphoto")]
    public MessagePicture NextPhoto()
    {
        return _messagePictureImporter.NextPicture();
    }

}