using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;


namespace ExcelTool.Commands.FreightCharges
{
    public class FreightChargeCommand : IRequest<MemoryStream>
    {
        [FromForm(Name = "freightChargeFile")]
        public IFormFile _空海运差异比例文件 { get; set; }
    }
}
