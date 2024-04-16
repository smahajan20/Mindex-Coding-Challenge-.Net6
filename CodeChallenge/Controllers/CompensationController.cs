using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            if (compensation == null)
            {
                return NotFound();
            }

            if(compensation.EmployeeId == null)
            {
                return NotFound("Employee Id is null");
            }

            _logger.LogDebug($"Received compensation create request");

            _compensationService.Create(compensation);

            return CreatedAtRoute("getByEmployeeId", new { employeeId = compensation.EmployeeId }, compensation);
        }

        [HttpGet("{employeeId}", Name = "getByEmployeeId")]
        public IActionResult GetByEmployeeId(String employeeId)
        {
            _logger.LogDebug($"Received employee get request for '{employeeId}'");

            var compensation = _compensationService.GetByEmployeeId(employeeId);

            if (compensation == null)
                return NotFound($"Compensation for the employee with {employeeId} could not be found");

            return Ok(compensation);
        }
    }
}
