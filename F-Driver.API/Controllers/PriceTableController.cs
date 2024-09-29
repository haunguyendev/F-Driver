using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace F_Driver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceTableController : ControllerBase
    {
        private readonly PriceTableService _priceTableService;

        public PriceTableController(PriceTableService priceTableService)
        {
            _priceTableService = priceTableService;
        }

        //Get price table by id
        [HttpGet("{priceTableId}")]
        public async Task<IActionResult> GetPriceTableById(int priceTableId)
        {
            var priceTable = await _priceTableService.GetPriceTableById(priceTableId);
            if (priceTable == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Price table not found."));
                return NotFound(result);
            }
            return Ok(ApiResult<PriceTableResponse>.Succeed(new PriceTableResponse { PriceTable = priceTable }));
        }

        //Create price table
        [HttpPost]
        public async Task<IActionResult> CreateNewPriceTable([FromBody] PriceTableRequest priceTableRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var priceTable = await _priceTableService.CreatePriceTable(priceTableRequest.MapToTableModel());
            if (!priceTable)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Create price table is unsuccessful."));
                return BadRequest(result);
            }
                return Created();

        }

        //Update price table
        [HttpPut("{priceTableId}")]
        public async Task<IActionResult> UpdatePriceTable(int priceTableId, [FromBody] PriceTableRequest priceTableRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var priceTable = await _priceTableService.UpdatePriceTable(priceTableId, priceTableRequest.MapToTableModel());
            if (priceTable == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Price table not found."));
                return NotFound(result);
            }
            return Ok(ApiResult<PriceTableResponse>.Succeed(new PriceTableResponse { PriceTable = priceTable }));
        }
    }
}
