using ExcelTool.Common;
using ExcelTool.Models.EmployedStatistics;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace ExcelTool.Commands.EmployedStatistics
{
    public class EmployedStatisticUploadOtherCommandHandler : IRequestHandler<EmployedStatisticUploadOtherCommand, List<EmployedStatistic>>
    {
        private readonly IStaticFileSetting fileSetting;

        public EmployedStatisticUploadOtherCommandHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public static int GetStatistic(string value)
        {
            int res;
            if (int.TryParse(value, out res))
            {
                return res;
            }
            return 0;
        }


        public async Task<List<EmployedStatistic>> Handle(EmployedStatisticUploadOtherCommand request, CancellationToken cancellationToken)
        {
            var list = new List<EmployedStatistic>();
            var currentTmpFolder = Path.Combine(fileSetting.TmpFolder, Guid.NewGuid().ToString());
            if (!Directory.Exists(currentTmpFolder))
            {
                Directory.CreateDirectory(currentTmpFolder);
            }
            var filePaths = request.Files.Select(f =>
            {
                var a = 1;
                var fileName = f.FileName;
                string pattern = @"\d{4}年";
                var year = "未知";
                if (Regex.IsMatch(fileName, pattern))
                {
                    var ms = Regex.Match(fileName, pattern);
                    year = ms.Value.Substring(0, ms.Value.Length - 1);
                }
                return Path.Combine(currentTmpFolder, year + ".xlsx");
            }).ToList();

            if (filePaths.Count > 0)
            {
                for (var idx = 0; idx < filePaths.Count; idx++)
                {
                    var filePath = filePaths[idx];
                    using (var targetStream = File.Create(filePath))
                    {
                        await request.Files[idx].CopyToAsync(targetStream);
                        targetStream.Close();
                        var fs = File.OpenRead(filePath);
                        IWorkbook wk = new XSSFWorkbook(fs);

                        ISheet sheet = wk.GetSheetAt(0);
                        IRow row;
                        var columnMap = new _企业人员情况ColumnMap();
                        int dataStartRowIndex = 0;
                        //定位数据行以及列头映射
                        for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                        {
                            row = sheet.GetRow(rowIndex);
                            var c = sheet.GetRow(rowIndex).GetCell(0);
                            if (c == null) { continue; }
                            var isTotalRow = Regex.IsMatch(c.ToString(), @"合计");
                            //合计行向上5行内找列头映射
                            if (isTotalRow)
                            {
                                //看一下下一行是否是英文的total,确定数据的起始行
                                var isEnTotalRow = Regex.IsMatch(sheet.GetRow(rowIndex + 1).GetCell(0).ToString().ToLower(), @"total");
                                if (isEnTotalRow)
                                {
                                    dataStartRowIndex = rowIndex + 2;
                                }
                                else
                                {
                                    dataStartRowIndex = rowIndex + 1;
                                }

                                for (int i = 1; i <= 5; i++)
                                {
                                    var tRowIdx = rowIndex - i;
                                    for (int columnIndex = 1; columnIndex < row.LastCellNum; columnIndex++)
                                    {
                                        var v = sheet.GetRow(tRowIdx).GetCell(columnIndex).ToString();

                                        if (Regex.IsMatch(v, @"年末从业"))
                                        {
                                            columnMap._年末从业人员 = columnIndex;
                                        }
                                        if (Regex.IsMatch(v, @"大专以"))
                                        {
                                            columnMap._大专以上 = columnIndex;
                                        }
                                        if (Regex.IsMatch(v, @"中高级"))
                                        {
                                            columnMap._中高级职称 = columnIndex;
                                        }
                                        if (Regex.IsMatch(v, @"留学归"))
                                        {
                                            columnMap._留学归国人员 = columnIndex;
                                        }
                                        if (Regex.IsMatch(v, @"外籍常"))
                                        {
                                            columnMap._外籍常驻人员 = columnIndex;
                                        }
                                        switch (v)
                                        {

                                        }
                                        Console.WriteLine(v.ToString());
                                    }
                                }
                                break;
                            }
                        }
                        for (int i = dataStartRowIndex; i <= sheet.LastRowNum; i++)
                        {
                            row = sheet.GetRow(i);  //读取当前行数据
                            var data = new EmployedStatistic();
                            var c = row.GetCell(0);
                            if (c == null) { continue; }
                            data._名称 = c.ToString().Trim();
                            int year;
                            var yearStr = filePath.Substring(filePath.Length - 9, 4);
                            if (int.TryParse(yearStr, out year))
                            {
                                data._年度 = year;
                            }
                            data._国内公司 = data._名称.ExistHanziCharacter();
                            if (columnMap._中高级职称 > 0)
                            {
                                string value = row.GetCell(columnMap._中高级职称).ToString();
                                data._中高级职称 = GetStatistic(value);
                            }

                            if (columnMap._外籍常驻人员 > 0)
                            {
                                string value = row.GetCell(columnMap._外籍常驻人员).ToString();
                                data._外籍常驻人员 = GetStatistic(value);
                            }

                            if (columnMap._大专以上 > 0)
                            {
                                string value = row.GetCell(columnMap._大专以上).ToString();
                                data._大专以上 = GetStatistic(value);
                            }

                            if (columnMap._年末从业人员 > 0)
                            {
                                string value = row.GetCell(columnMap._年末从业人员).ToString();
                                data._年末从业人员 = GetStatistic(value);
                            }

                            if (columnMap._留学归国人员 > 0)
                            {
                                string value = row.GetCell(columnMap._留学归国人员).ToString();
                                data._留学归国人员 = GetStatistic(value);
                            }

                            list.Add(data);
                        }

                    }
                }
                Directory.Delete(currentTmpFolder,true);
            }

            return list;
        }
    }


    class _企业人员情况ColumnMap
    {
        public int _年末从业人员 { get; set; }
        public int _大专以上 { get; set; }
        public int _中高级职称 { get; set; }
        public int _留学归国人员 { get; set; }
        public int _外籍常驻人员 { get; set; }
    }
}
