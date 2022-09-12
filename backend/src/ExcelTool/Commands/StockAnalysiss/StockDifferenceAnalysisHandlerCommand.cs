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
using Npoi.Mapper;
using OfficeOpenXml.Style;

namespace ExcelTool.Commands.StockAnalysiss
{
    public class StockDifferenceAnalysisHandlerCommand : IRequestHandler<StockDifferenceAnalysisCommand, MemoryStream>
    {
        private readonly IStaticFileSetting fileSetting;

        public StockDifferenceAnalysisHandlerCommand(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public async Task<MemoryStream> Handle(StockDifferenceAnalysisCommand request, CancellationToken cancellationToken)
        {
            var currentTmpFolder = fileSetting.GenerateTemporaryFolder();
            var list源数据 = new List<_库存差异源数据>();
            var dict部门源数据 = new Dictionary<string, List<_库存差异源数据>>();
            var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");
            var dict分析结果 = new Dictionary<string, _分析结果>();

            #region 读取明细表
            if (request._明细 != null)
            {
                using (var targetStream = File.Create(filePath))
                {
                    await request._明细.CopyToAsync(targetStream);
                    targetStream.Close();
                    var mapper = new Mapper(filePath);
                    var sheetDatas = mapper.Take<_库存差异源数据>().Select(x => x.Value).ToList();
                    //sheetDatas.ForEach(it =>
                    //{
                    //    it._店铺 = it._店铺.Substring(0, it._店铺.LastIndexOf("-")).Trim();
                    //});
                    list源数据.AddRange(sheetDatas);
                }
            }
            #endregion

            #region 数据分析
            var list部门名称 = list源数据.Select(x => x._部门).Distinct().ToList();
            list部门名称.Add("未匹配");

            list部门名称.ForEach(depName =>
            {
                var res1 = new _分析结果();
                res1._部门 = depName;
                res1._统计时间 = request.StatisticalTime;
                res1.type = _结果类型._南棠ERP可用库存大于亚马逊后台库存;
                dict分析结果.Add($"{depName}{ res1.type}", res1);

                var res2 = new _分析结果();
                res2._部门 = depName;
                res2._统计时间 = request.StatisticalTime;
                res2.type = _结果类型._南棠ERP可用库存小于亚马逊后台库存;
                dict分析结果.Add($"{depName}{ res2.type}", res2);
            });

            list源数据.ForEach(item =>
            {
                _结果类型 type = item._差异数量 > 0 ? _结果类型._南棠ERP可用库存大于亚马逊后台库存 : _结果类型._南棠ERP可用库存小于亚马逊后台库存;
                var depName = string.IsNullOrEmpty(item._部门) ? "未匹配" : item._部门;
                var dictKey = $"{item._部门}{type}";
                _分析结果 result = dict分析结果[dictKey];
                result.SKU.Add(item.SKU);
                result._店铺.Add(item._店铺);
                result._差异数量 += item._差异数量;
                result._差异总额 += item._差异总额;

                var source = dict部门源数据.ContainsKey(depName) ? dict部门源数据[depName] : new List<_库存差异源数据>();
                source.Add(item);
                dict部门源数据[depName] = source;
            });
            #endregion

            #region 打印结果
            var exportFolder = fileSetting.GenerateTemporaryFolder("export");

            if (list源数据.Count > 0)
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var workbox = package.Workbook;
                    list部门名称.ForEach(depName =>
                    {
                        if (dict部门源数据.ContainsKey(depName))
                        {
                            var sheet = workbox.Worksheets.Add($"{depName}差异数据");
                            _生成部门源数据Sheet(sheet, dict部门源数据[depName]);
                        }
                    });

                    var list分析结果 = dict分析结果.Values.ToList().Where(x => x._部门 != "未匹配").ToList();
                    var sheetOfAnalysisSource = workbox.Worksheets.Add($"透视源数据");
                    _生成透视源数据Sheet(sheetOfAnalysisSource, list分析结果);

                    var sheetOfAnalysis = workbox.Worksheets.Add($"各部门库存差异情况");
                    _生成透视Sheet(sheetOfAnalysis, list部门名称, dict分析结果);
                    package.Save();
                }
            }
            #endregion

            var memoryStream = new MemoryStream();
            using (var fs = File.OpenRead(filePath))
                fs.CopyTo(memoryStream);
            Directory.Delete(currentTmpFolder, true);
            return memoryStream;
        }

        private static void _生成部门源数据Sheet(ExcelWorksheet sheet, List<_库存差异源数据> list)
        {
            sheet.Cells[1, 1].Value = "店铺";
            sheet.Cells[1, 2].Value = "ASIN";
            sheet.Cells[1, 3].Value = "MSKU";
            sheet.Cells[1, 4].Value = "FNSKU";
            sheet.Cells[1, 5].Value = "SKU";
            sheet.Cells[1, 6].Value = "中文品名";
            sheet.Cells[1, 7].Value = "后台可售";
            sheet.Cells[1, 8].Value = "预留数量";
            sheet.Cells[1, 9].Value = "新系统可用库存";
            sheet.Cells[1, 10].Value = "新系统库存数量";
            sheet.Cells[1, 11].Value = "订单占用库存";
            sheet.Cells[1, 12].Value = "签收未入库";
            sheet.Cells[1, 13].Value = "后台不可售";
            sheet.Cells[1, 14].Value = "差异数量";
            sheet.Cells[1, 15].Value = "成本单价";
            sheet.Cells[1, 16].Value = "头程单价";
            sheet.Cells[1, 17].Value = "差异成本";
            sheet.Cells[1, 18].Value = "差异头程";
            sheet.Cells[1, 19].Value = "差异总额";
            sheet.Cells[1, 20].Value = "部门";
            sheet.Cells[1, 21].Value = "组别";
            sheet.Cells[1, 22].Value = "运营";

            if (list != null)
            {
                for (int idx = 0, rowIndex = 2; idx < list.Count; idx++)
                {
                    var data = list[idx];
                    sheet.Cells[rowIndex, 1].Value = data._店铺;
                    sheet.Cells[rowIndex, 2].Value = data.ASIN;
                    sheet.Cells[rowIndex, 3].Value = data.MSKU;
                    sheet.Cells[rowIndex, 4].Value = data.FNSKU;
                    sheet.Cells[rowIndex, 5].Value = data.SKU;
                    sheet.Cells[rowIndex, 6].Value = data._中文品名;
                    sheet.Cells[rowIndex, 7].Value = data._后台可售;
                    sheet.Cells[rowIndex, 8].Value = data._预留数量;
                    sheet.Cells[rowIndex, 9].Value = data._新系统可用库存;
                    sheet.Cells[rowIndex, 10].Value = data._新系统库存数量;
                    sheet.Cells[rowIndex, 11].Value = data._订单占用库存;
                    sheet.Cells[rowIndex, 12].Value = data._签收未入库;
                    sheet.Cells[rowIndex, 13].Value = data._后台不可售;
                    sheet.Cells[rowIndex, 14].Value = data._差异数量;
                    sheet.Cells[rowIndex, 15].Value = data._成本单价;
                    sheet.Cells[rowIndex, 16].Value = data._头程单价;
                    sheet.Cells[rowIndex, 17].Value = data._差异成本;
                    sheet.Cells[rowIndex, 18].Value = data._差异头程;
                    sheet.Cells[rowIndex, 19].Value = data._差异总额;
                    sheet.Cells[rowIndex, 20].Value = data._部门;
                    sheet.Cells[rowIndex, 21].Value = data._组别;
                    sheet.Cells[rowIndex, 22].Value = data._运营;
                    rowIndex++;
                }
            }

        }

        private static void _生成透视源数据Sheet(ExcelWorksheet sheet, List<_分析结果> list)
        {
            sheet.Cells[1, 1].Value = "部门";
            sheet.Cells[1, 2].Value = "差异数量";
            sheet.Cells[1, 3].Value = "SKU个数";
            sheet.Cells[1, 4].Value = "店铺个数";
            sheet.Cells[1, 5].Value = "差异总额";
            sheet.Cells[1, 6].Value = "差异类型";
            sheet.Cells[1, 7].Value = "统计时间";
            for (int idx = 0, rowIndex = 2; idx < list.Count; idx++)
            {
                var data = list[idx];
                sheet.Cells[rowIndex, 1].Value = data._部门;
                sheet.Cells[rowIndex, 2].Value = data._差异数量;
                sheet.Cells[rowIndex, 3].Value = data.SKU个数;
                sheet.Cells[rowIndex, 4].Value = data._店铺个数;
                sheet.Cells[rowIndex, 5].Value = data._差异总额;
                sheet.Cells[rowIndex, 6].Value = data.type;
                sheet.Cells[rowIndex, 7].Value = data._统计时间.ToString("yyyy/MM/dd");
                rowIndex++;
            }
        }

        private static void _生成透视Sheet(ExcelWorksheet sheet, List<string> list部门名称, Dictionary<string, _分析结果> dict)
        {
            sheet.Cells[2, 2].Value = "差异数量";
            sheet.Cells[2, 3].Value = "SKU个数";
            sheet.Cells[2, 4].Value = "店铺个数";
            sheet.Cells[2, 5].Value = "差异总额";
            sheet.Cells[2, 6].Value = "差异数量";
            sheet.Cells[2, 7].Value = "SKU个数";
            sheet.Cells[2, 8].Value = "店铺个数";
            sheet.Cells[2, 9].Value = "差异总额";

            using (var rng = sheet.Cells[1, 1, 2, 1])
            {
                rng.Merge = true;
                rng.Value = "部门";
            }

            using (var rng = sheet.Cells[1, 2, 1, 5])
            {
                rng.Merge = true;
                rng.Value = "南棠ERP可用库存大于亚马逊后台库存";
            }

            using (var rng = sheet.Cells[1, 6, 1, 9])
            {
                rng.Merge = true;
                rng.Value = "南棠ERP可用库存小于于亚马逊后台库存";
            }

            using (var rng = sheet.Cells[1, 10, 2, 10])
            {
                rng.Merge = true;
                rng.Value = "统计时间";
            }

            using (var rng = sheet.Cells[1, 1, 2, 10])
            {
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
            }

            var cw = 11;
            sheet.Column(1).Width = 18;
            sheet.Column(2).Width = cw;
            sheet.Column(3).Width = cw;
            sheet.Column(4).Width = cw;
            sheet.Column(5).Width = cw;
            sheet.Column(6).Width = cw;
            sheet.Column(7).Width = cw;
            sheet.Column(8).Width = cw;
            sheet.Column(9).Width = cw;
            sheet.Column(10).Width = cw;

            sheet.Row(1).Height = 30;
            int startRowIndex = 3;
            int rowIndex = startRowIndex;

            for (int idx = 0; idx < list部门名称.Count; idx++)
            {
                var depName = list部门名称[idx];
                if (depName != "未匹配")
                {
                    var res1 = dict[$"{depName}{_结果类型._南棠ERP可用库存大于亚马逊后台库存}"];
                    var res2 = dict[$"{depName}{_结果类型._南棠ERP可用库存小于亚马逊后台库存}"];

                    sheet.Cells[rowIndex, 1].Value = depName;
                    sheet.Cells[rowIndex, 2].Value = res1._差异数量;
                    sheet.Cells[rowIndex, 3].Value = res1.SKU个数;
                    sheet.Cells[rowIndex, 4].Value = res1._店铺个数;
                    sheet.Cells[rowIndex, 5].Value = res1._差异总额;

                    sheet.Cells[rowIndex, 6].Value = res2._差异数量;
                    sheet.Cells[rowIndex, 7].Value = res2.SKU个数;
                    sheet.Cells[rowIndex, 8].Value = res2._店铺个数;
                    sheet.Cells[rowIndex, 9].Value = res2._差异总额;
                    sheet.Cells[rowIndex, 10].Value = res1._统计时间.ToString("yyyy/MM/dd");
                    rowIndex++;
                }
            }
            sheet.Cells[rowIndex, 1].Value = "总计";
            sheet.Cells[rowIndex, 2].Formula = $"SUM(B{startRowIndex}:B{rowIndex - 1})";
            sheet.Cells[rowIndex, 3].Formula = $"SUM(C{startRowIndex}:C{rowIndex - 1})";
            sheet.Cells[rowIndex, 4].Formula = $"SUM(D{startRowIndex}:D{rowIndex - 1})";
            sheet.Cells[rowIndex, 5].Formula = $"SUM(E{startRowIndex}:E{rowIndex - 1})";
            sheet.Cells[rowIndex, 6].Formula = $"SUM(F{startRowIndex}:F{rowIndex - 1})";
            sheet.Cells[rowIndex, 7].Formula = $"SUM(G{startRowIndex}:G{rowIndex - 1})";
            sheet.Cells[rowIndex, 8].Formula = $"SUM(H{startRowIndex}:H{rowIndex - 1})";
            sheet.Cells[rowIndex, 9].Formula = $"SUM(I{startRowIndex}:I{rowIndex - 1})";
            using (var rng = sheet.Cells[1, 1, rowIndex, 10])
            {
                rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            }

            using (var rng = sheet.Cells[1, 1, 2, 10])
            {
                var colFromHex = System.Drawing.ColorTranslator.FromHtml("#DA9694");
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.Font.Bold = true;
            }

            using (var rng = sheet.Cells[2, 2, 2, 9])
            {
                var colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
            }

            using (var rng = sheet.Cells[rowIndex, 1, rowIndex, 10])
            {
                var colFromHex = System.Drawing.ColorTranslator.FromHtml("#B8CCE4");
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.Font.Bold = true;
            }
        }
    }

    public enum _结果类型
    {
        _南棠ERP可用库存大于亚马逊后台库存,
        _南棠ERP可用库存小于亚马逊后台库存
    }

    public class _分析结果
    {
        public _结果类型 type { get; set; }
        public string _部门 { get; set; }
        public int _差异数量 { get; set; }
        public decimal _差异总额 { get; set; }
        public DateTime _统计时间 { get; set; }
        public int SKU个数 { get { return SKU.Count; } }
        public int _店铺个数 { get { return _店铺.Count; } }

        public HashSet<string> _店铺 { get; set; } = new HashSet<string>();
        public HashSet<string> SKU { get; set; } = new HashSet<string>();
    }
}
