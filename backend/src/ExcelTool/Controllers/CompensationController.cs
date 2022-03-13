using ExcelTool.Commands.Compensations;
using ExcelTool.Commands.EmployedStatistics;
using ExcelTool.Commands.StockAnalysiss;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ExcelTool.Controllers
{
    /// <summary>
    /// 亚马逊退货赔偿订单
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CompensationController : ControllerBase
    {
        private readonly IMediator mediator;

        public CompensationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload([FromForm] CompensationAnalysisUploadCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
