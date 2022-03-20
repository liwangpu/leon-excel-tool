using ExcelTool.Models.StockAnalysiss;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ExcelTool.Commands.StockAnalysiss
{
    public class StockAnalysisUploadCommand : IRequest
    {
        [FromForm(Name = "detailFiles")]
        public List<IFormFile> _总账明细 { get; set; }
        [FromForm(Name = "skuMappingFiles")]
        public List<IFormFile> SKU匹配文件 { get; set; }
        [FromForm(Name = "saleOrderFiles")]
        public List<IFormFile> _销售流水 { get; set; }
    }
}
