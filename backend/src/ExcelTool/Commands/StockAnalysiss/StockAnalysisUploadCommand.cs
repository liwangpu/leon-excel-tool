using ExcelTool.Models.StockAnalysiss;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ExcelTool.Commands.StockAnalysiss
{
    public class StockAnalysisUploadCommand : IRequest<List<_库存出入总账>>
    {
        [FromForm(Name = "detailFiles")]
        public List<IFormFile> DetailFiles { get; set; }
        [FromForm(Name = "skuMappingFiles")]
        public List<IFormFile> SKUMappingFiles { get; set; }
    }
}
