using ExcelTool.Common;
using MediatR;
using OfficeOpenXml;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExcelTool.Commands.EmployedStatistics
{
    public class EmployedStatisticOtherExportHandler : IRequestHandler<EmployedStatisticOtherExport, MemoryStream>
    {
        private readonly IStaticFileSetting fileSetting;

        public EmployedStatisticOtherExportHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public async Task<MemoryStream> Handle(EmployedStatisticOtherExport request, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(fileSetting.TmpFolder, Guid.NewGuid() + ".xlsx");
            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                var workbox = package.Workbook;
                var sheet1 = workbox.Worksheets.Add("Sheet1");

                #region 标题行
                sheet1.Cells[1, 1].Value = "公司";
                sheet1.Cells[1, 2].Value = "年度";
                sheet1.Cells[1, 3].Value = "年末从业人员";
                sheet1.Cells[1, 4].Value = "大专以上";
                sheet1.Cells[1, 5].Value = "中高级职称";
                sheet1.Cells[1, 6].Value = "留学归国人员";
                sheet1.Cells[1, 7].Value = "外籍常驻人员";
                #endregion

                for (int idx = 0, rowIndex = 2; idx < request.Datas.Count; idx++)
                {
                    var data = request.Datas[idx];
                    sheet1.Cells[rowIndex, 1].Value = data._名称;
                    sheet1.Cells[rowIndex, 2].Value = data._年度;
                    sheet1.Cells[rowIndex, 3].Value = data._年末从业人员;
                    sheet1.Cells[rowIndex, 4].Value = data._大专以上;
                    sheet1.Cells[rowIndex, 5].Value = data._中高级职称;
                    sheet1.Cells[rowIndex, 6].Value = data._留学归国人员;
                    sheet1.Cells[rowIndex, 7].Value = data._外籍常驻人员;
                    rowIndex++;
                }
                package.Save();
            }
            var memoryStream = new MemoryStream();
            using (var fs = File.OpenRead(filePath))
                fs.CopyTo(memoryStream);
            File.Delete(filePath);
            return memoryStream;
        }
    }
}
