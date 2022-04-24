using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;


namespace ExcelTool.Commands.StockAnalysiss
{
    public class StockDifferenceAnalysisCommand : IRequest<MemoryStream>
    {
        [FromForm(Name = "detailFile")]
        public IFormFile _明细 { get; set; }
        [FromForm(Name = "statisticalTime")]
        public DateTime StatisticalTime { get; set; }
    }
}
