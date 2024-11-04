using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.API.Payloads.Response;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.Helpers;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace F_Driver.API.Controllers
{
    [Route("api/pricetables")]
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
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Create price table is unsuccessful or fromZoneId and toZoneId is duplicate"));
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
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Price table not found or fromZoneId and toZoneId is duplicate"));
                return NotFound(result);
            }
            return Ok(ApiResult<PriceTableResponse>.Succeed(new PriceTableResponse { PriceTable = priceTable }));
        }

        //Get price table by ZoneFrom and ZoneTo
        [HttpGet("from/{zoneFromId}/to/{zoneToId}")]
        public async Task<IActionResult> GetByZoneFromAndZoneTo(int zoneToId, int zoneFromId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var priceTable = await _priceTableService.GetPriceTableByZoneFromAndZoneTo(zoneFromId, zoneToId);
            if (priceTable == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Price table not found."));
                return NotFound(result);
            }    
            return Ok(ApiResult<PriceTableResponse>.Succeed(new PriceTableResponse { PriceTable = priceTable }));
        }

        //Delete price table
        [HttpDelete("{priceTableId}")]
        public async Task<IActionResult> DeletePriceTable(int priceTableId)
        {
            var priceTable = await _priceTableService.GetPriceTableById(priceTableId);
            if (priceTable == null)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Price table not found."));
                return NotFound(result);
            }
            await _priceTableService.DeletePriceTable(priceTableId);
            return NoContent();
        }
        #region
        [HttpGet]
        [SwaggerOperation(
       Summary = "Get all price tables",
       Description = "Returns a list of all price tables with optional filters, sorting, and pagination."
   )]
        [SwaggerResponse(200, "Price tables retrieved successfully", typeof(PaginatedList<PriceTableModel>))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(500, "An error occurred while retrieving price tables")]
        public async Task<IActionResult> GetAllPriceTables([FromQuery] PriceTableQueryParams parameters)
        {
            try
            {
                // Gọi service để lấy danh sách phân trang và lọc theo các tiêu chí
                var priceTables = await _priceTableService.GetAllPriceTablesAsync(parameters);

                return Ok(ApiResult<PaginatedList<PriceTableModel>>.Succeed(priceTables));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResult<string>.Error(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<string>.Error(ex.Message));
            }
        }
        #endregion
    }
}
