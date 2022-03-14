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

            var list退货订单 = new List<_退货订单>();
            var list赔偿订单 = new List<_赔偿订单>();
            var list退货处理方案 = new List<_退货处理方案>();
            var list赔偿处理方案 = new List<_赔偿处理方案>();
            var list部门匹配 = new List<_部门匹配>();
            var list部门 = new List<string>();

            #region 读取退货订单
            if (request._退货订单 != null)
            {
                for (var idx = request._退货订单.Count - 1; idx >= 0; idx--)
                {
                    var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString()}.xlsx");
                    using (var targetStream = File.Create(filePath))
                    {
                        await request._退货订单[idx].CopyToAsync(targetStream);
                        targetStream.Close();
                        using (var package = new ExcelPackage(new FileInfo(filePath)))
                        {
                            for (var sdx = package.Workbook.Worksheets.Count - 1; sdx >= 0; sdx--)
                            {
                                var sheet = package.Workbook.Worksheets[sdx];
                                var sheetDatas = SheetReader<_退货订单>.From(sheet);
                                sheetDatas.ForEach(it =>
                                {
                                    it._店铺 = it._店铺.Substring(0, it._店铺.LastIndexOf("-")).Trim();
                                });
                                list退货订单.AddRange(sheetDatas);
                            }
                        }
                    }
                }
            }
            #endregion

            #region 读取赔偿订单
            if (request._赔偿订单 != null)
            {
                for (var idx = request._赔偿订单.Count - 1; idx >= 0; idx--)
                {
                    var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString()}.xlsx");
                    using (var targetStream = File.Create(filePath))
                    {
                        await request._赔偿订单[idx].CopyToAsync(targetStream);
                        targetStream.Close();
                        using (var package = new ExcelPackage(new FileInfo(filePath)))
                        {
                            for (var sdx = package.Workbook.Worksheets.Count - 1; sdx >= 0; sdx--)
                            {
                                var sheet = package.Workbook.Worksheets[sdx];
                                var sheetDatas = SheetReader<_赔偿订单>.From(sheet);
                                sheetDatas.ForEach(it =>
                                {
                                    it._店铺 = it._店铺.Substring(0, it._店铺.LastIndexOf("-")).Trim();
                                });
                                list赔偿订单.AddRange(sheetDatas);
                            }
                        }
                    }
                }
            }
            #endregion

            #region 读取处理方案匹配表
            if (request._处理方案 != null)
            {
                var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString()}.xlsx");
                using (var targetStream = File.Create(filePath))
                {
                    await request._处理方案.CopyToAsync(targetStream);
                    targetStream.Close();
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        for (var sdx = package.Workbook.Worksheets.Count - 1; sdx >= 0; sdx--)
                        {
                            var sheet = package.Workbook.Worksheets[sdx];
                            if (sheet.Name == "赔偿处理方案")
                            {
                                var sheetDatas = SheetReader<_赔偿处理方案>.From(sheet);
                                list赔偿处理方案.AddRange(sheetDatas);
                            }
                            else if (sheet.Name == "退货处理方案")
                            {
                                var sheetDatas = SheetReader<_退货处理方案>.From(sheet);
                                list退货处理方案.AddRange(sheetDatas);
                            }
                            else { }
                        }
                    }
                }
            }
            #endregion

            #region 读取部门匹配表
            if (request._部门匹配表 != null)
            {
                var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString()}.xlsx");
                using (var targetStream = File.Create(filePath))
                {
                    await request._部门匹配表.CopyToAsync(targetStream);
                    targetStream.Close();
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        for (var sdx = package.Workbook.Worksheets.Count - 1; sdx >= 0; sdx--)
                        {
                            var sheet = package.Workbook.Worksheets[sdx];
                            var sheetDatas = SheetReader<_部门匹配>.From(sheet);
                            sheetDatas.ForEach(it =>
                            {
                                string pattern = @"[一二三四五六七八九十]{1,2}部";
                                var m = Regex.Match(it._负责人部门, pattern);
                                if (m.Success)
                                {
                                    list部门.Add(m.Value);
                                    it._部门 = m.Value;
                                }
                            });
                            list部门匹配.AddRange(sheetDatas);
                            list部门 = list部门.Select(n => n).Distinct().ToList();
                        }
                    }
                }
            }
            #endregion

            #region 数据处理
            // 需要erp处理的库存属性
            var list退货单需要EPR处理的 = list退货处理方案.Where(x => x._需要ERP操作).ToList();
            var list退货单需要后台处理的 = list退货处理方案.Where(x => x._需要后台操作).ToList();

            list退货订单.ForEach(it =>
            {
                // 匹配部门
                var dep = list部门匹配.Find(x => x._店铺名 == it._店铺);
                it._部门 = dep != null ? dep._部门 : "未匹配";

                // 匹配操作
                var a1 = list退货单需要EPR处理的.FirstOrDefault(x => x.DetailedDisposition == it._库存属性);
                it._需要ERP操作 = a1 != null;
                it._对应的ERP操作 = a1 != null ? a1._ERP执行动作 : null;
                var a2 = list退货单需要后台处理的.FirstOrDefault(x => x.DetailedDisposition == it._库存属性);
                it._需要后台操作 = a2 != null;
                it._对应的后台操作 = a2 != null ? a2._后台执行动作 : null;
            });

            var list赔偿单需要EPR处理的 = list赔偿处理方案.Where(x => x._需要ERP操作).ToList();
            var list赔偿单需要后台处理的 = list赔偿处理方案.Where(x => x._需要后台操作).ToList();

            // 赔偿订单匹配部门
            list赔偿订单.ForEach(it =>
            {
                var dep = list部门匹配.Find(x => x._店铺名 == it._店铺);
                it._部门 = dep != null ? dep._部门 : "未匹配";
                if (it._原因 == "Damaged_Warehouse")
                {
                    var aaa = 1;
                }
                // 匹配操作
                var a1 = list赔偿单需要EPR处理的.FirstOrDefault(x => x.reason == it._原因);
                it._需要ERP操作 = a1 != null;
                it._对应的ERP操作 = a1 != null ? a1._ERP执行动作 : null;
                var a2 = list赔偿单需要后台处理的.FirstOrDefault(x => x.reason == it._原因);
                it._需要后台操作 = a2 != null;
                it._对应的后台操作 = a2 != null ? a2._后台执行动作 : null;
            });

            // 添加未匹配部门
            list部门.Add("未匹配");
            #endregion

            Directory.Delete(currentTmpFolder, true);
            var exportFolder = fileSetting.GenerateTemporaryFolder("export");

            #region 打印退货单
            var exp退货订单Path = Path.Combine(exportFolder, "亚马逊退货订单.xlsx");

            using (ExcelPackage package = new ExcelPackage(new FileInfo(exp退货订单Path)))
            {
                var workbox = package.Workbook;
                list部门.ForEach(dName =>
                {
                    var _list部门退货订单 = list退货订单.Where(x => x._部门 == dName).ToList();
                    // ERP操作表
                    {
                        var sheet = workbox.Worksheets.Add($"{dName} ERP");
                        var datas = _list部门退货订单.Where(x => x._需要ERP操作).ToList();
                        _生成退货订单Sheet(sheet, datas, "ERP");
                    }

                    // 后台操作表
                    {
                        var sheet = workbox.Worksheets.Add($"{dName} 亚马逊后台");
                        var datas = _list部门退货订单.Where(x => x._需要后台操作).ToList();
                        _生成退货订单Sheet(sheet, datas, "亚马逊后台");
                    }
                });

                package.Save();
            }
            #endregion

            #region 打印赔偿单
            var exp赔偿订单Path = Path.Combine(exportFolder, "亚马逊赔偿订单.xlsx");
            using (ExcelPackage package = new ExcelPackage(new FileInfo(exp赔偿订单Path)))
            {
                var workbox = package.Workbook;
                list部门.ForEach(dName =>
                {
                    var _list部门赔偿订单 = list赔偿订单.Where(x => x._部门 == dName).ToList();

                    // ERP操作表
                    {
                        var sheet = workbox.Worksheets.Add($"{dName} ERP");
                        var datas = _list部门赔偿订单.Where(x => x._需要ERP操作).ToList();
                        _生成赔偿订单Sheet(sheet, datas, "ERP");
                    }

                    // 后台操作表
                    {
                        var sheet = workbox.Worksheets.Add($"{dName} 亚马逊后台");
                        var datas = _list部门赔偿订单.Where(x => x._需要后台操作).ToList();
                        _生成赔偿订单Sheet(sheet, datas, "亚马逊后台");
                    }
                });
                package.Save();
            }
            #endregion

            return null;
        }

        private static void _生成退货订单Sheet(ExcelWorksheet sheet, List<_退货订单> list, string operate)
        {
            #region 标题行
            sheet.Cells[1, 1].Value = "店铺";
            sheet.Cells[1, 2].Value = "国家";
            sheet.Cells[1, 3].Value = "品名";
            sheet.Cells[1, 4].Value = "sku";
            sheet.Cells[1, 5].Value = "分类";
            sheet.Cells[1, 6].Value = "品牌";
            sheet.Cells[1, 7].Value = "订单号";
            sheet.Cells[1, 8].Value = "商品名称";
            sheet.Cells[1, 9].Value = "MSKU";
            sheet.Cells[1, 10].Value = "ASIN";
            sheet.Cells[1, 11].Value = "数量";
            sheet.Cells[1, 12].Value = "发货仓库编号";
            sheet.Cells[1, 13].Value = "库存属性";
            sheet.Cells[1, 14].Value = "退货原因";
            sheet.Cells[1, 15].Value = "状态";
            sheet.Cells[1, 16].Value = "LPN编号";
            sheet.Cells[1, 17].Value = "买家备注";
            sheet.Cells[1, 18].Value = "退货时间";
            sheet.Cells[1, 19].Value = "订购时间";
            sheet.Cells[1, 20].Value = "备注";
            sheet.Cells[1, 21].Value = "操作";
            #endregion

            #region 数据行
            for (int idx = 0, rowIndex = 2; idx < list.Count; idx++)
            {
                var data = list[idx];
                sheet.Cells[rowIndex, 1].Value = data._店铺;
                sheet.Cells[rowIndex, 2].Value = data._国家;
                sheet.Cells[rowIndex, 3].Value = data._品名;
                sheet.Cells[rowIndex, 4].Value = data.SKU;
                sheet.Cells[rowIndex, 5].Value = data._分类;
                sheet.Cells[rowIndex, 6].Value = data._品牌;
                sheet.Cells[rowIndex, 7].Value = data._订单号;
                sheet.Cells[rowIndex, 8].Value = data._商品名称;
                sheet.Cells[rowIndex, 9].Value = data.MSKU;
                sheet.Cells[rowIndex, 10].Value = data.ASIN;
                sheet.Cells[rowIndex, 11].Value = data._数量;
                sheet.Cells[rowIndex, 12].Value = data._发货仓库编号;
                sheet.Cells[rowIndex, 13].Value = data._库存属性;
                sheet.Cells[rowIndex, 14].Value = data._退货原因;
                sheet.Cells[rowIndex, 15].Value = data._状态;
                sheet.Cells[rowIndex, 16].Value = data.LPN编号;
                sheet.Cells[rowIndex, 17].Value = data._买家备注;
                sheet.Cells[rowIndex, 18].Value = data._退货时间;
                sheet.Cells[rowIndex, 19].Value = data._订购时间;
                sheet.Cells[rowIndex, 20].Value = data._备注;
                string action = null;
                switch (operate)
                {
                    case "ERP":
                        action = data._对应的ERP操作;
                        break;
                    default:
                        action = data._对应的后台操作;
                        break;
                }
                sheet.Cells[rowIndex, 21].Value = action;
                rowIndex++;
            }
            #endregion

            var list数据透视 = new List<_退货赔偿订单数据透视>();
            // 数据透视
            list.ForEach(it =>
            {
                var dt = list数据透视.FirstOrDefault(d => d._店铺 == it._店铺);
                if (dt == null)
                {
                    dt = new _退货赔偿订单数据透视() { _店铺 = it._店铺 };
                    list数据透视.Add(dt);
                }
                var asinDt = dt.Items.FirstOrDefault(d => d.Key == it.MSKU);
                if (asinDt == null)
                {
                    asinDt = new ASINInfo() { Key = it.MSKU, _数量 = it._数量 };
                    dt.Items.Add(asinDt);
                }
                else
                {
                    asinDt._数量 += it._数量;
                }

                dt._总数量合计 += it._数量;
            });

            _生成数据透视信息(sheet, list数据透视);
        }

        private static void _生成赔偿订单Sheet(ExcelWorksheet sheet, List<_赔偿订单> list, string operate)
        {
            #region 标题行
            sheet.Cells[1, 1].Value = "店铺";
            sheet.Cells[1, 2].Value = "国家";
            sheet.Cells[1, 3].Value = "时间";
            sheet.Cells[1, 4].Value = "赔偿编号";
            sheet.Cells[1, 5].Value = "订单号";
            sheet.Cells[1, 6].Value = "原因";
            sheet.Cells[1, 7].Value = "MSKU";
            sheet.Cells[1, 8].Value = "FNSKU";
            sheet.Cells[1, 9].Value = "ASIN";
            sheet.Cells[1, 10].Value = "标题";
            sheet.Cells[1, 11].Value = "状况";
            sheet.Cells[1, 12].Value = "币种";
            sheet.Cells[1, 13].Value = "每件商品赔偿金额";
            sheet.Cells[1, 14].Value = "总金额";
            sheet.Cells[1, 15].Value = "赔偿数量（现金）";
            sheet.Cells[1, 16].Value = "赔偿数量（库存）";
            sheet.Cells[1, 17].Value = "赔偿数量（总计）";
            sheet.Cells[1, 18].Value = "原始赔偿编号";
            sheet.Cells[1, 19].Value = "原始赔偿类型";
            sheet.Cells[1, 20].Value = "操作";
            #endregion

            #region 数据行
            for (int idx = 0, rowIndex = 2; idx < list.Count; idx++)
            {
                var data = list[idx];
                sheet.Cells[rowIndex, 1].Value = data._店铺;
                sheet.Cells[rowIndex, 2].Value = data._国家;
                sheet.Cells[rowIndex, 3].Value = data._时间;
                sheet.Cells[rowIndex, 4].Value = data._赔偿编号;
                sheet.Cells[rowIndex, 5].Value = data._订单号;
                sheet.Cells[rowIndex, 6].Value = data._原因;
                sheet.Cells[rowIndex, 7].Value = data.MSKU;
                sheet.Cells[rowIndex, 8].Value = data.FNSKU;
                sheet.Cells[rowIndex, 9].Value = data.ASIN;
                sheet.Cells[rowIndex, 10].Value = data._标题;
                sheet.Cells[rowIndex, 11].Value = data._状况;
                sheet.Cells[rowIndex, 12].Value = data._币种;
                sheet.Cells[rowIndex, 13].Value = data._每件商品赔偿金额;
                sheet.Cells[rowIndex, 14].Value = data._总金额;
                sheet.Cells[rowIndex, 15].Value = data._赔偿数量_现金;
                sheet.Cells[rowIndex, 16].Value = data._赔偿数量_库存;
                sheet.Cells[rowIndex, 17].Value = data._赔偿数量_总计;
                sheet.Cells[rowIndex, 18].Value = data._原始赔偿编号;
                sheet.Cells[rowIndex, 19].Value = data._原始赔偿类型;
                string action = null;
                switch (operate)
                {
                    case "ERP":
                        action = data._对应的ERP操作;
                        break;
                    default:
                        action = data._对应的后台操作;
                        break;
                }
                sheet.Cells[rowIndex, 20].Value = action;
                rowIndex++;
            }
            #endregion

            // 数据透视
            var list数据透视 = new List<_退货赔偿订单数据透视>();
            list.ForEach(it =>
            {
                var dt = list数据透视.FirstOrDefault(d => d._店铺 == it._店铺);
                if (dt == null)
                {
                    dt = new _退货赔偿订单数据透视() { _店铺 = it._店铺 };
                    list数据透视.Add(dt);
                }
                var asinDt = dt.Items.FirstOrDefault(d => d.Key == it.MSKU);
                if (asinDt == null)
                {
                    asinDt = new ASINInfo() { Key = it.MSKU, _数量 = it._赔偿数量_总计, _金额 = it._总金额 };
                    dt.Items.Add(asinDt);
                }
                else
                {
                    asinDt._数量 += it._赔偿数量_总计;
                    asinDt._金额 += it._总金额;
                }

                dt._总数量合计 += it._赔偿数量_总计;
                dt._总金额合计 += it._总金额;

                if (dt._总金额合计 != dt.Items.Select(x => x._金额).Sum())
                {
                    var bbb = 1;
                }
            });

            _生成数据透视信息(sheet, list数据透视);
        }

        private static void _生成数据透视信息(ExcelWorksheet sheet, List<_退货赔偿订单数据透视> list)
        {
            sheet.Cells[1, 24].Value = "店铺";
            sheet.Cells[1, 25].Value = "MSKU";
            sheet.Cells[1, 26].Value = "数量";
            sheet.Cells[1, 27].Value = "金额";
            sheet.Cells[1, 28].Value = "总数量";
            sheet.Cells[1, 29].Value = "总金额";

            for (int idx = 0, rowIndex = 2; idx < list.Count; idx++)
            {
                var data = list[idx];
                var startIndex = rowIndex;
                for (var ssdx = data.Items.Count - 1; ssdx >= 0; ssdx--)
                {
                    var it = data.Items[ssdx];

                    sheet.Cells[rowIndex, 25].Value = it.Key;
                    sheet.Cells[rowIndex, 26].Value = it._数量;
                    sheet.Cells[rowIndex, 27].Value = it._金额;
                    sheet.Cells[rowIndex, 28].Value = data._总数量合计;
                    if (ssdx == data.Items.Count - 1)
                    {
                        sheet.Cells[rowIndex, 24].Value = data._店铺;
                        sheet.Cells[rowIndex, 29].Value = data._总金额合计;
                    }
                    if (ssdx == 0)
                    {
                        if (rowIndex - startIndex > 0)
                        {
                            using (var rng = sheet.Cells[startIndex, 24, rowIndex, 24])
                            {
                                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                                rng.Merge = true;
                            }

                            using (var rng = sheet.Cells[startIndex, 28, rowIndex, 28])
                            {
                                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                                rng.Merge = true;
                            }
                            using (var rng = sheet.Cells[startIndex, 29, rowIndex, 29])
                            {
                                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                                rng.Merge = true;
                            }
                        }
                    }
                    rowIndex++;

                }

                if (idx == list.Count - 1)
                {
                    using (var rng = sheet.Cells[1, 24, rowIndex - 1, 29])
                    {
                        rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    }

                    using (var rng = sheet.Cells[1, 24, 1, 27])
                    {
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                        rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                    }
                    sheet.Column(24).Width = 30;
                    sheet.Column(25).Width = 22;
                    sheet.Column(26).Width = 10;
                    sheet.Column(27).Width = 10;
                    sheet.Column(28).Width = 10;
                    sheet.Column(29).Width = 10;
                    sheet.Column(26).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Column(27).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Column(28).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Column(29).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }
        }
    }

    class _退货赔偿订单数据透视
    {
        public string _店铺 { get; set; }
        public List<ASINInfo> Items { get; set; } = new List<ASINInfo>();
        public decimal _总数量合计 { get; set; }
        public decimal _总金额合计 { get; set; }
    }

    class ASINInfo
    {
        public string Key { get; set; }
        public decimal _数量 { get; set; }
        public decimal _金额 { get; set; }
    }
}
