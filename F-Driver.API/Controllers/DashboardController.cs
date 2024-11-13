using F_Driver.API.Common;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace F_Driver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                var dashboardSummary = await _dashboardService.GetDashboardSummaryAsync();
                return Ok(ApiResult<DashboardResponseModel>.Succeed(dashboardSummary));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<object>.Fail(ex));
            }
        }

        [HttpGet("weekly-statistics")]
        public async Task<IActionResult> GetWeeklyStatistics(int month, int year)
        {
            if (month < 1 || month > 12 || year < 1)
            {
                return BadRequest("Invalid month or year.");
            }

            var result = await _dashboardService.GetWeeklyStatisticsAsync(month, year);
            return Ok(result);
        }
    }
}
