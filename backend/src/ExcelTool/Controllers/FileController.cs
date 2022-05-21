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
    public class FileController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IStaticFileSetting fileSetting;

        public FileController(IMediator mediator, IStaticFileSetting fileSetting)
        {
            this.mediator = mediator;
            this.fileSetting = fileSetting;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload([FromForm] FileUploadCommand command)
        {
            await mediator.Send(command);
            return Ok();
        }

        [HttpGet("Download/{fileKey}")]
        public async Task<IActionResult> Download(string fileKey)
        {
            var filePath = Path.Combine(fileSetting.AttactmentFolder, fileKey);
            var memoryStream = new MemoryStream();
            using (var fs = System.IO.File.OpenRead(filePath))
                fs.CopyTo(memoryStream);
            return File(memoryStream.ToArray(), "application/octet-stream", fileKey);
        }

        [HttpGet("Check/{fileKey}")]
        public async Task<IActionResult> CheckFileExist(string fileKey)
        {
            var exist = await mediator.Send(new FileExistCheckQuery(fileKey));
            return Ok(exist);
        }
    }
}
