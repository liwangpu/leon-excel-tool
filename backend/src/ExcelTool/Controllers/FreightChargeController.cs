using ExcelTool.Commands.FreightCharges;
using ExcelTool.Commands.StockAnalysiss;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExcelTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreightChargeController : ControllerBase
    {
        private readonly IMediator mediator;

        public FreightChargeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("FreightChargeAnylysis")]
        public async Task<IActionResult> FreightChargeAnylysis([FromForm] FreightChargeCommand command)
        {
            var result = await mediator.Send(command);
            return File(result.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx");
        }

        [HttpPost("DifferenceAnylysis")]
        public async Task<IActionResult> DifferenceAnylysis([FromForm] StockDifferenceAnalysisCommand command)
        {
            var result = await mediator.Send(command);
            return File(result.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx");
        }
    }
}
