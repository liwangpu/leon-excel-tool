using ExcelTool.Commands.EmployedStatistics;
using ExcelTool.Commands.StockAnalysiss;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ExcelTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockAnalysisController : ControllerBase
    {
        private readonly IMediator mediator;

        public StockAnalysisController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload([FromForm] StockAnalysisUploadCommand command)
        {
            await mediator.Send(command);
            return Ok();
        }
    }
}
