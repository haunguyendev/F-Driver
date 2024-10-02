using F_Driver.API.Common;
using F_Driver.API.Payloads.Request;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace F_Driver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripRequestController : ControllerBase
    {
        private readonly TripRequestService _tripRequestService;

        public TripRequestController(TripRequestService tripRequestService)
        {
            _tripRequestService = tripRequestService;
        }

        //Create trip request
        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] TripRequestRequest tripRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tripRequest = await _tripRequestService.CreateTripRequest(tripRequestModel.MapToTripRequestModel());
            if (!tripRequest)
            {
                var result = ApiResult<Dictionary<string, string[]>>.Fail(new Exception("Create trip request is unsuccessful."));
                return BadRequest(result);
            }
            return Created();
        }
    }
}
