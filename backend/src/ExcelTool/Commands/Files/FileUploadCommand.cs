using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExcelTool.Commands.Files
{
    public class FileUploadCommand : IRequest
    {
        [FromForm(Name = "fileKey")]
        public string FileKey { get; set; }
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
