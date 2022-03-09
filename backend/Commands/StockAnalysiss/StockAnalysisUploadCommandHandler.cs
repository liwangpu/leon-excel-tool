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

            var datas = new List<_库存出入总账>();
            if (filePaths.Count > 0)
            {
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
                //Directory.Delete(currentTmpFolder, true);
            }

            var exportFilePath = Path.Combine(fileSetting.TmpFolder, Guid.NewGuid() + ".xlsx");
            using (ExcelPackage package = new ExcelPackage(new FileInfo(exportFilePath)))
            {
                var workbox = package.Workbook;
                var sheet1 = workbox.Worksheets.Add("Sheet1");

                #region 标题行
                sheet1.Cells[1, 1].Value = "仓库";
                sheet1.Cells[1, 2].Value = "店铺";
                sheet1.Cells[1, 3].Value = "SKU";
                sheet1.Cells[1, 4].Value = "仓库店铺SKU";
                sheet1.Cells[1, 5].Value = "部门";
                sheet1.Cells[1, 6].Value = "运营";
                sheet1.Cells[1, 7].Value = "产品标题";
                sheet1.Cells[1, 8].Value = "期初数量";
                sheet1.Cells[1, 9].Value = "期末数量";
                sheet1.Cells[1, 10].Value = "期初金额";
                sheet1.Cells[1, 11].Value = "期末金额";
                sheet1.Cells[1, 12].Value = "入库数量";
                sheet1.Cells[1, 13].Value = "入库金额";
                sheet1.Cells[1, 14].Value = "出库数量";
                sheet1.Cells[1, 15].Value = "出库金额";
                sheet1.Cells[1, 16].Value = "销售金额";
                sheet1.Cells[1, 17].Value = "本期平均销售出库金额";
                sheet1.Cells[1, 18].Value = "周转率";
                sheet1.Cells[1, 19].Value = "本期低周转";
                sheet1.Cells[1, 20].Value = "上期低周转";
                sheet1.Cells[1, 21].Value = "本期滞销库存";
                sheet1.Cells[1, 22].Value = "上期滞销库存";
                #endregion

                #region 数据行
                if (datas.Count > 0)
                {
                    for (int idx = 0, rowIndex = 2; idx < datas.Count; idx++)
                    {
                        var data = datas[idx];
                        sheet1.Cells[rowIndex, 1].Value = data._仓库;
                        sheet1.Cells[rowIndex, 2].Value = data._店铺;
                        sheet1.Cells[rowIndex, 3].Value = data.SKU;
                        sheet1.Cells[rowIndex, 4].Value = data._仓库店铺SKU;
                        //sheet1.Cells[rowIndex, 5].Value = data._部门;
                        sheet1.Cells[rowIndex, 6].Value = data._运营;
                        sheet1.Cells[rowIndex, 7].Value = data._产品标题;
                        sheet1.Cells[rowIndex, 8].Value = data._期初数量;
                        sheet1.Cells[rowIndex, 9].Value = data._期末数量;
                        sheet1.Cells[rowIndex, 10].Value = data._期初金额;
                        sheet1.Cells[rowIndex, 11].Value = data._期末金额;
                        sheet1.Cells[rowIndex, 12].Value = data._入库数量;
                        sheet1.Cells[rowIndex, 13].Value = data._入库金额;
                        sheet1.Cells[rowIndex, 14].Value = data._出库数量;
                        sheet1.Cells[rowIndex, 15].Value = data._出库金额;
                        sheet1.Cells[rowIndex, 16].Value = data._销售金额;
                        //sheet1.Cells[rowIndex, 17].Value = data._本期平均销售出库金额;
                        //sheet1.Cells[rowIndex, 18].Value = data._周转率;
                        //sheet1.Cells[rowIndex, 19].Value = data._本期低周转;
                        //sheet1.Cells[rowIndex, 20].Value = data._上期低周转;
                        //sheet1.Cells[rowIndex, 21].Value = data._本期滞销库存;
                        //sheet1.Cells[rowIndex, 22].Value = data._上期滞销库存;
                        //sheet1.Cells[rowIndex, 23].Value = data._仓库;
                        rowIndex++;
                    }
                }
                #endregion
                //for (int idx = 0, rowIndex = 2; idx < request.Datas.Count; idx++)
                //{
                //    var data = request.Datas[idx];
                //    sheet1.Cells[rowIndex, 1].Value = data._名称;
                //    sheet1.Cells[rowIndex, 2].Value = data._年度;
                //    sheet1.Cells[rowIndex, 3].Value = data._年末从业人员;
                //    sheet1.Cells[rowIndex, 4].Value = data._大专以上;
                //    sheet1.Cells[rowIndex, 5].Value = data._中高级职称;
                //    sheet1.Cells[rowIndex, 6].Value = data._留学归国人员;
                //    sheet1.Cells[rowIndex, 7].Value = data._外籍常驻人员;
                //    rowIndex++;
                //}
                package.Save();
            }

            return new List<_库存出入总账>();
        }
    }
}
