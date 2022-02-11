using InvictaInternalAPI.Exceptions;
using InvictaInternalAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InvictaInternalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ILogger<SupplierController> _logger;
        private readonly ICancelService _service;

        public SupplierController(ILogger<SupplierController> logger, ICancelService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("supplierId")]
        public IActionResult GetOrderstoXlsFile(int supplierId)
        {
            _logger.LogInformation("Executing endpoint GetOrderstoXlsFile/SupplierController");
            _logger.LogInformation("Extracting information");
            if(supplierId == 8)
            {
                var data = _service.GetOrderJegSons();
                _logger.LogInformation("Extraction completed");
                return Ok(data);
                
            }else if(supplierId == 7)
            {
                var data = _service.GetOrderDesignerEyes();
                _logger.LogInformation("Extraction completed");
                return Ok(data);
            }
            else
            {
                _logger.LogError($"Invalid supplier: {supplierId}");
                throw new BusinessException($"Invalid supplier: {supplierId}");
            }
            
        }
    }
}
