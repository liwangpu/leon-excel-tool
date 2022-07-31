using ExcelTool.Domain.Models.AmazonCompensations;
using ExcelTool.Domain.Models.Commons;
using ExcelTool.Domain.Models.FreightCharges;
using ExcelTool.Domain.Utils;
using Npoi.Mapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelTool.Domain.Handler
{
    public class FreightChargeHandler
    {
        protected string path空海运差异文件;
        protected string folder临时文件夹;

        #region ctor
        protected FreightChargeHandler()
        {

        }
        public FreightChargeHandler(string p空海运差异文件, string f临时文件夹)
                  : this()
        {
            path空海运差异文件 = p空海运差异文件;
            folder临时文件夹 = f临时文件夹;
        }
        #endregion

        public async Task<MemoryStream> Handle()
        {
            var list空海运差异数据 = new List<_空海运差异数据>();
            var groupMap = new Dictionary<string, List<_空海运差异数据>>();

            if (!string.IsNullOrEmpty(path空海运差异文件))
            {
                var mapper = new Mapper(path空海运差异文件);
                var sheetDatas = mapper.Take<_空海运差异数据>("Sheet2").Select(x => x.Value).ToList();
                string lastGroupKey = "";
                sheetDatas.ForEach(it =>
                {
                    //有些时候有汇总行
                    if (string.IsNullOrWhiteSpace(it.DATE发件日期) && string.IsNullOrWhiteSpace(it.CONS运单号))
                    {
                        return;
                    }
                    List<_空海运差异数据> group;
                    if (it._应付 > 0 || (it._预估运费 == 0 && it._应付 == 0))
                    {
                        lastGroupKey = Guid.NewGuid().ToString("N");
                        group = new List<_空海运差异数据>();
                        groupMap.Add(lastGroupKey, group);
                    }
                    else
                    {
                        groupMap.TryGetValue(lastGroupKey, out group);
                    }
                    it.group = lastGroupKey;
                    group.Add(it);
                    list空海运差异数据.Add(it);
                });

                var aa = 1;
                groupMap.ForEach(kv =>
                {
                    var items = kv.Value;
                    var _预估总运费 = items.Sum(x => x._预估运费);
                    var _实际总运费 = items[0]._应付;
                    items.ForEach(it =>
                    {
                        if (items.Count > 1)
                        {
                            it._占比 = _预估总运费 > 0 ? Math.Round((it._预估运费 * 10000000) / (_预估总运费 * 10000000), 4) : 1;
                        }
                        else
                        {
                            it._占比 = 1;
                        }
                        it._实际应付 = Math.Round(it._占比 * _实际总运费, 2);
                    });
                });
            }

            var exportFilePath = Path.Combine(folder临时文件夹, "result.xlsx");
            #region 导出数据
            using (var package = new ExcelPackage(new FileInfo(exportFilePath)))
            {
                var workbox = package.Workbook;
                var sheet = workbox.Worksheets.Add($"空海运差异报表");
                var dataStartRowIdex = 2;

                #region 标题列
                using (var rng = sheet.Cells[1, 1, 1, 2])
                {
                    rng.Merge = true;
                    rng.Value = "货代账单数据";
                }
                using (var rng = sheet.Cells[1, 1, 2, 2])
                {
                    var colFromHex = System.Drawing.ColorTranslator.FromHtml("#9BC2E6");
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                }
                using (var rng = sheet.Cells[1, 3, 1, 6])
                {
                    rng.Merge = true;
                    rng.Value = "我司账单数据";
                }
                using (var rng = sheet.Cells[1, 3, 2, 6])
                {
                    var colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFE699");
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                }
                using (var rng = sheet.Cells[1, 7, 1, 10])
                {
                    rng.Merge = true;
                    rng.Value = "对比";
                }
                using (var rng = sheet.Cells[1, 7, 2, 11])
                {
                    var colFromHex = System.Drawing.ColorTranslator.FromHtml("#F8CBAD");
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                }
                using (var rng = sheet.Cells[1, 1, 2, 11])
                {
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                    rng.Style.Font.Size = 12;
                }
                sheet.Cells[2, 1].Value = "DATE发件日期";
                sheet.Cells[2, 2].Value = "CONS运单号";
                sheet.Cells[2, 3].Value = "业务员";
                sheet.Cells[2, 4].Value = "组别";
                sheet.Cells[2, 5].Value = "头程调拨";
                sheet.Cells[2, 6].Value = "二程调拨";
                sheet.Cells[2, 7].Value = "应付";
                sheet.Cells[2, 8].Value = "预估运费";
                sheet.Cells[2, 9].Value = "实际应付";
                sheet.Cells[2, 10].Value = "占比";
                sheet.Cells[2, 11].Value = "索赔";
                #endregion

                var rowIndex = dataStartRowIdex + 1;
                groupMap.ForEach(kv =>
                {
                    var items = kv.Value;
                    var rowStartIndex = rowIndex;
                    var dateStr = items[0].DATE发件日期;
                    var _实际总运费 = items[0]._应付;
                    items.ForEach(it =>
                    {
                        sheet.Cells[rowIndex, 1].Value = dateStr;
                        sheet.Cells[rowIndex, 2].Value = it.CONS运单号;
                        sheet.Cells[rowIndex, 3].Value = it._业务员;
                        sheet.Cells[rowIndex, 4].Value = it._组别;
                        sheet.Cells[rowIndex, 5].Value = it._头程调拨;
                        sheet.Cells[rowIndex, 6].Value = it._二程调拨;
                        sheet.Cells[rowIndex, 7].Value = it._应付;
                        sheet.Cells[rowIndex, 8].Value = it._预估运费;
                        sheet.Cells[rowIndex, 9].Value = it._实际应付;
                        sheet.Cells[rowIndex, 10].Value = it._占比;
                        sheet.Cells[rowIndex, 11].Value = it._索赔;
                        rowIndex += 1;
                    });
                    if (items.Count > 1)
                    {
                        using (var rng = sheet.Cells[rowStartIndex, 1, rowIndex - 1, 1])
                        {
                            rng.Merge = true;
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                            //rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                            rng.Style.WrapText = true;
                            rng.Value = dateStr;
                        }
                        using (var rng = sheet.Cells[rowStartIndex, 7, rowIndex - 1, 7])
                        {
                            rng.Merge = true;
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                            //rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                            rng.Style.WrapText = true;
                            rng.Value = _实际总运费;
                        }
                    }
                    else
                    {
                        sheet.Cells[rowStartIndex, 1, rowStartIndex, 1].Value = dateStr;
                        sheet.Cells[rowStartIndex, 7, rowStartIndex, 7].Value = _实际总运费;
                    }
                });

                sheet.Column(1).Width = 17;
                sheet.Column(2).Width = 17;
                sheet.Column(3).Width = 17;
                sheet.Column(4).Width = 17;
                sheet.Column(5).Width = 17;
                sheet.Column(6).Width = 17;
                sheet.Column(7).Width = 12;
                sheet.Column(8).Width = 12;
                sheet.Column(9).Width = 12;
                sheet.Column(10).Width = 12;
                sheet.Column(11).Width = 12;
                package.Save();
            }
            #endregion

            var memoryStream = new MemoryStream();
            using (var fs = File.OpenRead(exportFilePath))
                fs.CopyTo(memoryStream);
            File.Delete(exportFilePath);
            return memoryStream;
        }

    }
}
