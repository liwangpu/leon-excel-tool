using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ExcelTool.Commands.Compensations
{
    public class AmazonCompensationUploadCommand : IRequest<MemoryStream>
    {
        [FromForm(Name = "storeChangeName")]
        public IFormFile _店铺更名文件 { get; set; }
        [FromForm(Name = "compensations")]
        public IFormFile _赔偿订单 { get; set; }
    }
}
