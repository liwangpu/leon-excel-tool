using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;


namespace ExcelTool.Commands.AliExpress
{
    public class AlipayIntlBillCollectUploadCommand : IRequest<MemoryStream>
    {
        [FromForm(Name = "packages")]
        public List<IFormFile> Packages { get; set; }
    }
}
