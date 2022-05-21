using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ExcelTool.Commands.Compensations
{
    public class CompensationAnalysisUploadCommand : IRequest<MemoryStream>
    {
        [FromForm(Name = "refunds")]
        public IFormFile _退货订单 { get; set; }
        [FromForm(Name = "compensations")]
        public IFormFile _赔偿订单 { get; set; }
        [FromForm(Name = "solution")]
        public IFormFile _处理方案 { get; set; }
        [FromForm(Name = "departmentMap")]
        public IFormFile _部门匹配表 { get; set; }
        [FromForm(Name = "storeChangeName")]
        public IFormFile _店铺更名文件 { get; set; }
        [FromForm(Name = "msku2sku")]
        public IFormFile MSKU2SKU { get; set; }
        [FromForm(Name = "sku2Price")]
        public IFormFile SKU2Price { get; set; }
        [FromForm(Name = "exchangeRate")]
        public IFormFile _汇率匹配 { get; set; }
    }
}
