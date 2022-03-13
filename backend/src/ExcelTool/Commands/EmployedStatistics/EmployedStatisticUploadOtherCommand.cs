using ExcelTool.Models.EmployedStatistics;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ExcelTool.Commands.EmployedStatistics
{
    public class EmployedStatisticUploadOtherCommand : IRequest<List<EmployedStatistic>>
    {
        [FromForm(Name = "files")]
        public List<IFormFile> Files { get; set; }

        //[FromForm(Name = "version")]
        //public string Version { get; set; }
        //[FromForm(Name = "componentId")]
        //public string ComponentId { get; set; }
    }
}
