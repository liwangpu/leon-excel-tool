using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
    }
}
