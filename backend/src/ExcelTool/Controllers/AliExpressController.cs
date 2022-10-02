using ExcelTool.Commands.AliExpress;
using ExcelTool.Commands.Files;
using ExcelTool.Common;
using ExcelTool.Queries.Files;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ExcelTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AliExpressController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IStaticFileSetting fileSetting;

        public AliExpressController(IMediator mediator, IStaticFileSetting fileSetting)
        {
            this.mediator = mediator;
            this.fileSetting = fileSetting;
        }

        [HttpPost("AlipayIntlBillCollectUpload")]
        public async Task<IActionResult> AlipayIntlBillCollectUpload([FromForm] AlipayIntlBillCollectUploadCommand command)
        {
            var result = await mediator.Send(command);
            return File(result.ToArray(), "application/octet-stream", "result.xlsx");
        }

    }
}
