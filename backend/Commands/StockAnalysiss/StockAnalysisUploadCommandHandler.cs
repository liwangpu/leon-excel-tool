using ExcelTool.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExcelTool.Models.StockAnalysiss;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace ExcelTool.Commands.StockAnalysiss
{

    public class StockAnalysisUploadCommandHandler : IRequestHandler<StockAnalysisUploadCommand, List<_库存出入总账>>
    {

        private readonly IStaticFileSetting fileSetting;

        public StockAnalysisUploadCommandHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public async Task<List<_库存出入总账>> Handle(StockAnalysisUploadCommand request, CancellationToken cancellationToken)
        {
            var currentTmpFolder = Path.Combine(fileSetting.TmpFolder, Guid.NewGuid().ToString());
            if (!Directory.Exists(currentTmpFolder))
            {
                Directory.CreateDirectory(currentTmpFolder);
            }
            var daysMap = new Dictionary<string, int>();
            var filePaths = request.Files.Select(f =>
            {
                var fileName = f.FileName;
                string pattern = @"\d{1,3}天";
                int day = 0;
                if (Regex.IsMatch(fileName, pattern))
                {
                    var ms = Regex.Match(fileName, pattern);
                    var dayStr = ms.Value.Substring(0, ms.Value.Length - 1);
                    int.TryParse(dayStr, out day);
                }

                var pt = Path.Combine(currentTmpFolder, fileName);
                daysMap.Add(pt, day);
                return pt;
            }).ToList();

            if (filePaths.Count > 0)
            {
                var datas = new List<_库存出入总账>();
                for (var idx = 0; idx < filePaths.Count; idx++)
                {
                    var filePath = filePaths[idx];
                    using (var targetStream = File.Create(filePath))
                    {
                        int day = daysMap[filePath];
                        await request.Files[idx].CopyToAsync(targetStream);
                        targetStream.Close();
                        using (var package = new ExcelPackage(new FileInfo(filePath)))
                        {
                            for (var sdx = package.Workbook.Worksheets.Count - 1; sdx >= 0; sdx--)
                            {
                                var sheet = package.Workbook.Worksheets[sdx];
                                if (sheet.Name.ToLower() == "worksheet")
                                {
                                    var sheetDatas = SheetReader<_库存出入总账>.From(sheet);
                                    sheetDatas.ForEach(it =>
                                    {
                                        it._天数 = day;
                                    });
                                    datas.AddRange(sheetDatas);
                                }
                            }
                        }
                    }
                }
                Directory.Delete(currentTmpFolder, true);
                var a = 1;
            }

            return new List<_库存出入总账>();
        }
    }
}
