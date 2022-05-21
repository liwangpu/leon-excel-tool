using ExcelTool.Commands.Compensations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
            var commonFileName = "退货赔偿订单处理结果";
            if (command._退货订单 != null)
            {
                commonFileName = command._退货订单.FileName;
            }
            if (command._赔偿订单 != null)
            {
                commonFileName = command._赔偿订单.FileName;
            }
            return File(result.ToArray(), "application/octet-stream", commonFileName + ".xlsx");
        }

        [HttpPost("AmazonUpload")]
        public async Task<IActionResult> AmazonUpload([FromForm] AmazonCompensationUploadCommand command)
        {
            var result = await mediator.Send(command);
            var commonFileName = "亚马逊索赔处理结果";
            if (command._赔偿订单 != null)
            {
                commonFileName = command._赔偿订单.FileName;
            }
            return File(result.ToArray(), "application/octet-stream", commonFileName + ".xlsx");
        }
    }
}
