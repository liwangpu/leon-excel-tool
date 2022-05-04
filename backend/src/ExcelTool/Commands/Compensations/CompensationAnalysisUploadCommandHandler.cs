using ExcelTool.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using ExcelTool.Models.Compensations;
using OfficeOpenXml.Style;
using Npoi.Mapper;
using System.IO.Compression;

namespace ExcelTool.Commands.Compensations
{
    public class CompensationAnalysisUploadCommandHandler : IRequestHandler<CompensationAnalysisUploadCommand, MemoryStream>
    {
        private readonly IStaticFileSetting fileSetting;

        public CompensationAnalysisUploadCommandHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public async Task<MemoryStream> Handle(CompensationAnalysisUploadCommand request, CancellationToken cancellationToken)
        {
            var currentTmpFolder = fileSetting.GenerateTemporaryFolder();
            var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");
            return null;
        }
    }

}
