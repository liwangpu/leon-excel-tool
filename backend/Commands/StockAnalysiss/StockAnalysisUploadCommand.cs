using ExcelTool.Models.StockAnalysiss;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ExcelTool.Commands.StockAnalysiss
{
    public class StockAnalysisUploadCommand : IRequest<List<_库存出入总账>>
    {
        [FromForm(Name = "files")]
        public List<IFormFile> Files { get; set; }

        [FromForm(Name = "name")]
        public string Name { get; set; }
        //[FromForm(Name = "componentId")]
        //public string ComponentId { get; set; }
    }
}
