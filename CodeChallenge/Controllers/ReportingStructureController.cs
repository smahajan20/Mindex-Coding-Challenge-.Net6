using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/reporting")]
    public class ReportingStructureController : Controller
    {
        private readonly ILogger _logger;
        private readonly IReportingStructureService _reportingStructureService;

        public ReportingStructureController(ILogger<ReportingStructureController> logger, IReportingStructureService reportingStructureService)
        {
            _logger = logger;
            _reportingStructureService = reportingStructureService;
        }

        [HttpGet("{employeeId}", Name = "getNumberOfReports")]
        public IActionResult GetNumberOfReports(String employeeId)
        {
            _logger.LogDebug($"Received reporting structure get request for '{employeeId}'");

            var reportingStructure = _reportingStructureService.GetNumberOfReports(employeeId);

            if (reportingStructure == null)
                return NotFound();

            if (reportingStructure.Employee == null)
                return NotFound($"Employee with {employeeId} could not be found");

            return Ok(reportingStructure);
        }
    }
}
