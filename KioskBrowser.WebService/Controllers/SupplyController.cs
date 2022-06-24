using KioskBrowser.DataService.Data;
using KioskBrowser.DataService.Services;
using Microsoft.AspNetCore.Mvc;

namespace KioskBrowser.WebService.Controllers;

[ApiController]
public class SupplyController : ControllerBase
{
    
    private readonly DataCollectionService _dataService;
    private readonly ILogger<SupplyController> _logger;

    public SupplyController(DataCollectionService dataService, ILogger<SupplyController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    [HttpGet]
    [Route("Supply/GetProducts")]
    public IEnumerable<ProductData> GetProducts()
    {
        if (_dataService.ProductData is null) 
            return Enumerable.Empty<ProductData>();

        return _dataService.ProductData.All();
    }

    [HttpGet]
    [Route("Supply/GetGroups")]
    public IEnumerable<GroupData> GetGroups()
    {
        if (_dataService.GroupData is null)
            return Enumerable.Empty<GroupData>();

        return _dataService.GroupData.All();
    }

    [Route("Supply/Products/Add/{id?}")]
    public ProductData? Add(string id)
    {
        if (!Guid.TryParse(id, out var productId) || _dataService.ProductData is null)
            return default;

        var product = _dataService.ProductData.Get(productId);

        if (product is null)
            return default;

        product.TotalItems++;

        _dataService.ProductData.AddEdit(product);

        return product;
    }

    [Route("Supply/Products/Subtract/{id?}")]
    public ProductData? Subtract(string id)
    {
        if (!Guid.TryParse(id, out var productId) || _dataService.ProductData is null)
            return default;

        var product = _dataService.ProductData.Get(productId);

        if (product is null)
            return default;

        if (product.TotalItems > 0)
        {
            product.TotalItems--;
            _dataService.ProductData.AddEdit(product);
        }


        return product;
    }
}