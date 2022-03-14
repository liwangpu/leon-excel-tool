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
                                    it._店铺 = it._店铺.Substring(0, it._店铺.LastIndexOf("-"));
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
                                    it._店铺 = it._店铺.Substring(0, it._店铺.LastIndexOf("-"));
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
                it._需要ERP操作 = list退货单需要EPR处理的.Exists(x => x.DetailedDisposition == it._库存属性);
                it._需要后台操作 = list退货单需要后台处理的.Exists(x => x.DetailedDisposition == it._库存属性);
            });

            var list赔偿单需要EPR处理的 = list赔偿处理方案.Where(x => x._需要ERP操作).ToList();
            var list赔偿单需要后台处理的 = list赔偿处理方案.Where(x => x._需要后台操作).ToList();

            // 赔偿订单匹配部门
            list赔偿订单.ForEach(it =>
            {
                var dep = list部门匹配.Find(x => x._店铺名 == it._店铺);
                it._部门 = dep != null ? dep._部门 : "未匹配";
                // 匹配操作
                it._需要ERP操作 = list赔偿单需要EPR处理的.Exists(x => x.reason == it._原因);
                it._需要后台操作 = list赔偿单需要后台处理的.Exists(x => x.reason == it._原因);
            });

            // 添加未匹配部门
            list部门.Add("未匹配");
            #endregion


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
                        _生成退货订单Sheet(sheet, datas);
                    }

                    // 后台操作表
                    {
                        var sheet = workbox.Worksheets.Add($"{dName} 亚马逊后台");
                        var datas = _list部门退货订单.Where(x => x._需要后台操作).ToList();
                        _生成退货订单Sheet(sheet, datas);
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
                        _生成赔偿订单Sheet(sheet, datas);
                    }

                    // 后台操作表
                    {
                        var sheet = workbox.Worksheets.Add($"{dName} 亚马逊后台");
                        var datas = _list部门赔偿订单.Where(x => x._需要后台操作).ToList();
                        _生成赔偿订单Sheet(sheet, datas);
                    }
                });
                package.Save();
            }
            #endregion
            //var memoryStream = new MemoryStream();
            //using (var fs = File.OpenRead(exportFilePath))
            //    fs.CopyTo(memoryStream);
            //File.Delete(exportFilePath);
            return null;
        }

        private static void _生成退货订单Sheet(ExcelWorksheet sheet, List<_退货订单> list)
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
            #endregion

            #region 数据行
            //var datas = list退货订单.Where(x => x._部门 == dName).ToList();
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
                rowIndex++;
            }
            #endregion

            var list数据透视 = new List<_退货赔偿订单数据透视>();
            // 数据透视
            list.ForEach(it =>
            {
                var key = $"{it._店铺}{it.ASIN}";
                var dt = list数据透视.FirstOrDefault(d => d._店铺_ASIN == key);
                if (dt == null)
                {
                    dt = new _退货赔偿订单数据透视();
                    dt._数量 = it._数量;
                    dt._店铺 = it._店铺;
                    dt.ASIN = it.ASIN;
                    list数据透视.Add(dt);
                }
                else
                {
                    dt._数量 += it._数量;
                }
            });

            _生成数据透视信息(sheet, list数据透视);
        }

        private static void _生成赔偿订单Sheet(ExcelWorksheet sheet, List<_赔偿订单> list)
        {
            #region 标题行
            sheet.Cells[1, 1].Value = "国家";
            sheet.Cells[1, 2].Value = "时间";
            sheet.Cells[1, 3].Value = "赔偿编号";
            sheet.Cells[1, 4].Value = "订单号";
            sheet.Cells[1, 5].Value = "原因";
            sheet.Cells[1, 6].Value = "MSKU";
            sheet.Cells[1, 7].Value = "FNSKU";
            sheet.Cells[1, 8].Value = "ASIN";
            sheet.Cells[1, 9].Value = "标题";
            sheet.Cells[1, 10].Value = "状况";
            sheet.Cells[1, 11].Value = "币种";
            sheet.Cells[1, 12].Value = "每件商品赔偿金额";
            sheet.Cells[1, 13].Value = "总金额";
            sheet.Cells[1, 14].Value = "赔偿数量（现金）";
            sheet.Cells[1, 15].Value = "赔偿数量（库存）";
            sheet.Cells[1, 16].Value = "赔偿数量（总计）";
            sheet.Cells[1, 17].Value = "原始赔偿编号";
            sheet.Cells[1, 18].Value = "原始赔偿类型";
            #endregion

            #region 数据行
            for (int idx = 0, rowIndex = 2; idx < list.Count; idx++)
            {
                var data = list[idx];
                sheet.Cells[rowIndex, 1].Value = data._国家;
                sheet.Cells[rowIndex, 2].Value = data._时间;
                sheet.Cells[rowIndex, 3].Value = data._赔偿编号;
                sheet.Cells[rowIndex, 4].Value = data._订单号;
                sheet.Cells[rowIndex, 5].Value = data._原因;
                sheet.Cells[rowIndex, 6].Value = data.MSKU;
                sheet.Cells[rowIndex, 7].Value = data.FNSKU;
                sheet.Cells[rowIndex, 8].Value = data.ASIN;
                sheet.Cells[rowIndex, 9].Value = data._标题;
                sheet.Cells[rowIndex, 10].Value = data._状况;
                sheet.Cells[rowIndex, 11].Value = data._币种;
                sheet.Cells[rowIndex, 12].Value = data._每件商品赔偿金额;
                sheet.Cells[rowIndex, 13].Value = data._总金额;
                sheet.Cells[rowIndex, 14].Value = data._赔偿数量_现金;
                sheet.Cells[rowIndex, 15].Value = data._赔偿数量_库存;
                sheet.Cells[rowIndex, 16].Value = data._赔偿数量_总计;
                sheet.Cells[rowIndex, 17].Value = data._原始赔偿编号;
                sheet.Cells[rowIndex, 18].Value = data._原始赔偿类型;
                rowIndex++;
            }
            #endregion

            var list数据透视 = new List<_退货赔偿订单数据透视>();
            // 数据透视
            list.ForEach(it =>
            {
                var key = $"{it._店铺}{it.ASIN}";
                var dt = list数据透视.FirstOrDefault(d => d._店铺_ASIN == key);
                if (dt == null)
                {
                    dt = new _退货赔偿订单数据透视();
                    dt._数量 = it._赔偿数量_总计;
                    dt._店铺 = it._店铺;
                    dt.ASIN = it.ASIN;
                    list数据透视.Add(dt);
                }
                else
                {
                    dt._数量 += it._赔偿数量_总计;
                }
            });

            _生成数据透视信息(sheet, list数据透视);
        }

        private static void _生成数据透视信息(ExcelWorksheet sheet, List<_退货赔偿订单数据透视> list)
        {
            sheet.Cells[1, 24].Value = "店铺";
            sheet.Cells[1, 25].Value = "ANSIN";
            sheet.Cells[1, 26].Value = "数量";

            var list店铺名称 = list.Select(d => d._店铺).Distinct().ToList();
            for (int idx = 0, rowIndex = 2; idx < list店铺名称.Count; idx++)
            {
                var name = list店铺名称[idx];
                var startIndex = rowIndex;
                var datas = list.Where(d => d._店铺 == name).ToList();
                datas.ForEach(it =>
                {
                    sheet.Cells[rowIndex, 24].Value = it._店铺;
                    sheet.Cells[rowIndex, 25].Value = it.ASIN;
                    sheet.Cells[rowIndex, 26].Value = it._数量;
                    rowIndex++;
                });
                var endIndex = rowIndex - 1;
                if (endIndex - startIndex > 1)
                {
                    using (var rng = sheet.Cells[startIndex, 24, endIndex, 24])
                    {
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                        rng.Merge = true;
                    }
                }

                if (idx == list店铺名称.Count - 1)
                {
                    using (var rng = sheet.Cells[1, 24, endIndex, 26])
                    {
                        rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    }

                    using (var rng = sheet.Cells[1, 24, 1, 26])
                    {
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                        rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                    }
                    sheet.Column(24).Width = 30;
                    sheet.Column(25).Width = 22;
                    sheet.Column(26).Width = 10;


                }
            }
        }
    }

    class _退货赔偿订单数据透视
    {
        public string _店铺 { get; set; }
        public string ASIN { get; set; }
        public string _店铺_ASIN { get { return $"{_店铺}{ASIN}"; } }
        public decimal _数量 { get; set; }
    }
}
