using ExcelTool.Models.EmployedStatistics;
using MediatR;
using System.Collections.Generic;
using System.IO;

namespace ExcelTool.Commands.EmployedStatistics
{
    public class EmployedStatisticOtherExport : IRequest<MemoryStream>
    {
        public List<EmployedStatistic> Datas { get; set; }
    }
}
