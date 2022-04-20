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

            var list退货订单 = new List<_退货订单>();
            var list赔偿订单 = new List<_赔偿订单>();
            var list退货处理方案 = new List<_退货处理方案>();
            var list赔偿处理方案 = new List<_赔偿处理方案>();
            var list部门匹配 = new List<_部门匹配>();
            var list部门 = new List<string>();
            var commonFileName = "退货赔偿订单处理结果";

            #region 读取退货订单
            if (request._退货订单 != null)
            {
                var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");
                using (var targetStream = File.Create(filePath))
                {
                    await request._退货订单.CopyToAsync(targetStream);
                    targetStream.Close();
                    var mapper = new Mapper(filePath);
                    var sheetDatas = mapper.Take<_退货订单>().Select(x => x.Value).ToList();
                    sheetDatas.ForEach(it =>
                    {
                        it._店铺 = it._店铺.Substring(0, it._店铺.LastIndexOf("-")).Trim();
                    });
                    list退货订单.AddRange(sheetDatas);
                }
            }
            #endregion

            #region 读取赔偿订单
            if (request._赔偿订单 != null)
            {
                var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");
                using (var targetStream = File.Create(filePath))
                {
                    await request._赔偿订单.CopyToAsync(targetStream);
                    targetStream.Close();

                    var mapper = new Mapper(filePath);
                    var sheetDatas = mapper.Take<_赔偿订单>().Select(x => x.Value).ToList();
                    sheetDatas.ForEach(it =>
                     {
                         it._店铺 = it._店铺.Substring(0, it._店铺.LastIndexOf("-")).Trim();
                     });
                    list赔偿订单.AddRange(sheetDatas);
                }
            }
            #endregion

            #region 读取处理方案匹配表
            if (request._处理方案 != null)
            {
                var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");
                using (var targetStream = File.Create(filePath))
                {
                    await request._处理方案.CopyToAsync(targetStream);
                    targetStream.Close();

                    var mapper = new Mapper(filePath);
                    var data1 = mapper.Take<_赔偿处理方案>("赔偿处理方案").Select(x => x.Value).ToList();
                    list赔偿处理方案.AddRange(data1);


                    var data2 = mapper.Take<_退货处理方案>("退货处理方案").Select(x => x.Value).ToList();
                    list退货处理方案.AddRange(data2);
                }
            }
            #endregion

            #region 读取部门匹配表
            if (request._部门匹配表 != null)
            {
                var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");
                using (var targetStream = File.Create(filePath))
                {
                    await request._部门匹配表.CopyToAsync(targetStream);
                    targetStream.Close();

                    var mapper = new Mapper(filePath);
                    var sheetDatas = mapper.Take<_部门匹配>().Select(x => x.Value).ToList();
                    sheetDatas.ForEach(it =>
                    {
                        string pattern = @"[一二三四五六七八九十]{1,2}部";
                        var m = Regex.Match(it._负责人部门, pattern);
                        if (m.Success)
                        {
                            list部门.Add(m.Value);
                            it._部门 = m.Value;
                        }
                        else
                        {
                            it._部门 = "未匹配";
                            list部门.Add("未匹配");
                        }
                    });

                    list部门匹配.AddRange(sheetDatas);
                    // 添加未匹配部门
                    list部门.Add("未匹配");
                    list部门 = list部门.Select(n => n).Distinct().ToList();
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
                // 匹配操作
                var a1 = list赔偿单需要EPR处理的.FirstOrDefault(x => x.reason == it._原因);
                it._需要ERP操作 = a1 != null;
                it._对应的ERP操作 = a1 != null ? a1._ERP执行动作 : null;
                var a2 = list赔偿单需要后台处理的.FirstOrDefault(x => x.reason == it._原因);
                it._需要后台操作 = a2 != null;
                it._对应的后台操作 = a2 != null ? a2._后台执行动作 : null;
            });


            #endregion

            Directory.Delete(currentTmpFolder, true);
            var exportFolder = fileSetting.GenerateTemporaryFolder("export");

            #region 打印退货单
            if (request._退货订单 != null)
            {
                var exp退货订单Path = Path.Combine(exportFolder, request._退货订单.FileName);

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

                    var _list汇总订单 = list退货订单.OrderBy(x => x._部门).ToList();
                    // ERP操作表
                    {
                        var sheet = workbox.Worksheets.Add($"ERP汇总");
                        var datas = _list汇总订单.Where(x => x._需要ERP操作).ToList();
                        _生成退货订单Sheet(sheet, datas, "ERP", false);
                    }

                    // 后台操作表
                    {
                        var sheet = workbox.Worksheets.Add($"亚马逊后台汇总");
                        var datas = _list汇总订单.Where(x => x._需要后台操作).ToList();
                        _生成退货订单Sheet(sheet, datas, "亚马逊后台", false);
                    }

                    package.Save();
                }
            }
            #endregion

            #region 打印赔偿单
            if (request._赔偿订单 != null)
            {
                var exp赔偿订单Path = Path.Combine(exportFolder, request._赔偿订单.FileName);
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

                    // 打印汇总表格
                    var _list所有偿订单 = list赔偿订单.OrderBy(x => x._部门).ToList();

                    // ERP操作表
                    {
                        var sheet = workbox.Worksheets.Add($"ERP汇总");
                        var datas = _list所有偿订单.Where(x => x._需要ERP操作).ToList();
                        _生成赔偿订单Sheet(sheet, datas, "ERP", false);
                    }

                    // 后台操作表
                    {
                        var sheet = workbox.Worksheets.Add($"亚马逊后台汇总");
                        var datas = _list所有偿订单.Where(x => x._需要后台操作).ToList();
                        _生成赔偿订单Sheet(sheet, datas, "亚马逊后台", false);
                    }
                    package.Save();
                }
            }
            #endregion

            var zipPath = Path.Combine(fileSetting.TmpFolder, $"{commonFileName}.zip").Replace(".xlsx", "");
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(exportFolder, zipPath);

            var memoryStream = new MemoryStream();
            using (var fs = File.OpenRead(zipPath))
                fs.CopyTo(memoryStream);
            File.Delete(zipPath);
            return memoryStream;
        }

        private static void _生成退货订单Sheet(ExcelWorksheet sheet, List<_退货订单> list, string operate, bool showPivot = true)
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
            if (!showPivot)
            {
                sheet.Cells[1, 22].Value = "部门";
            }
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
                if (!showPivot)
                {
                    sheet.Cells[rowIndex, 22].Value = data._部门;
                }
                rowIndex++;
            }
            #endregion

            if (!showPivot) { return; }

            var list数据透视 = new List<_退货单数据透视>();
            // 数据透视
            list.ForEach(it =>
            {
                var dt = list数据透视.FirstOrDefault(d => d._店铺 == it._店铺);
                if (dt == null)
                {
                    dt = new _退货单数据透视() { _店铺 = it._店铺 };
                    list数据透视.Add(dt);
                }
                var asinDt = dt.Items.FirstOrDefault(d => d.MSKU == it.MSKU);
                if (asinDt == null)
                {
                    asinDt = new _退货单数据统计项() { MSKU = it.MSKU, _数量 = it._数量 };
                    dt.Items.Add(asinDt);
                }
                else
                {
                    asinDt._数量 += it._数量;
                }

                //dt._总数量合计 += it._数量;
            });

            sheet.Cells[1, 24].Value = "店铺";
            sheet.Cells[1, 25].Value = "MSKU";
            sheet.Cells[1, 26].Value = "数量";
            sheet.Cells[1, 27].Value = "数量合计";
            for (int idx = 0, rowIndex = 2; idx < list数据透视.Count; idx++)
            {
                var data = list数据透视[idx];
                var startIndex = rowIndex;

                for (var ssdx = data.Items.Count - 1; ssdx >= 0; ssdx--)
                {
                    var it = data.Items[ssdx];

                    sheet.Cells[rowIndex, 25].Value = it.MSKU;
                    //sheet.Cells[rowIndex, 26].Value = string.Join(",", it._赔偿编号);
                    sheet.Cells[rowIndex, 26].Value = it._数量;
                    //sheet.Cells[rowIndex, 28].Value = it._金额;
                    sheet.Cells[rowIndex, 27].Value = data._总数量合计;
                    if (ssdx == data.Items.Count - 1)
                    {
                        sheet.Cells[rowIndex, 24].Value = data._店铺;
                        //sheet.Cells[rowIndex, 29].Value = data._总金额合计;
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

                            using (var rng = sheet.Cells[startIndex, 27, rowIndex, 27])
                            {
                                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                                rng.Merge = true;
                            }
                        }
                    }
                    rowIndex++;

                }

                if (idx == list数据透视.Count - 1)
                {
                    using (var rng = sheet.Cells[1, 24, rowIndex - 1, 27])
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
                    sheet.Column(26).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Column(27).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }
        }

        private static void _生成赔偿订单Sheet(ExcelWorksheet sheet, List<_赔偿订单> list, string operate, bool showPivot = true)
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
            if (!showPivot)
            {
                sheet.Cells[1, 21].Value = "部门";
            }
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
                if (!showPivot)
                {
                    sheet.Cells[rowIndex, 21].Value = data._部门;
                }
                rowIndex++;
            }
            #endregion

            if (!showPivot) { return; }
            // 数据透视
            var list数据透视 = new List<_赔偿订单数据透视>();
            list.ForEach(it =>
            {
                var dt = list数据透视.FirstOrDefault(d => d._店铺 == it._店铺);
                if (dt == null)
                {
                    dt = new _赔偿订单数据透视() { _店铺 = it._店铺 };
                    list数据透视.Add(dt);
                }
                var item = dt.Items.FirstOrDefault(d => d.MSKU == it.MSKU && d._国家 == it._国家 && d._赔偿编号 == it._赔偿编号);
                if (item == null)
                {
                    item = new _赔偿订单统计项() { MSKU = it.MSKU, _国家 = it._国家, _赔偿编号 = it._赔偿编号, _数量 = it._赔偿数量_总计 };
                    dt.Items.Add(item);
                }
                else
                {
                    item._数量 += it._赔偿数量_总计;
                }

            });

            // 透视详情
            sheet.Cells[1, 35].Value = "店铺";
            sheet.Cells[1, 36].Value = "MSKU";
            sheet.Cells[1, 37].Value = "国家";
            sheet.Cells[1, 38].Value = "赔偿编号";
            sheet.Cells[1, 39].Value = "数量";
            for (int idx = 0, rowIndex = 2; idx < list数据透视.Count; idx++)
            {
                var data = list数据透视[idx];
                var startIndex = rowIndex;
                string lastMSKU = null;
                for (var ssdx = 0; ssdx < data.Items.Count; ssdx++)
                {
                    var it = data.Items[ssdx];
                    sheet.Cells[rowIndex, 36].Value = it.MSKU;
                    sheet.Cells[rowIndex, 37].Value = it._国家;
                    sheet.Cells[rowIndex, 38].Value = it._赔偿编号;
                    sheet.Cells[rowIndex, 39].Value = it._数量;
                    if (ssdx == 0)
                    {
                        sheet.Cells[rowIndex, 35].Value = data._店铺;
                    }
                    //if (lastMSKU != it.MSKU)
                    //{

                    //    var skuStartIndex = data.Items.FindIndex(d => d.MSKU == it.MSKU);
                    //    var skuEndIndex = data.Items.FindLastIndex(d => d.MSKU == it.MSKU);
                    //    if (skuStartIndex != skuEndIndex)
                    //    {
                    //        try
                    //        {
                    //            sheet.Cells[startIndex + skuStartIndex, 40].Value = "0";
                    //            sheet.Cells[startIndex + skuEndIndex, 40].Value = "1";
                    //            using (var rng = sheet.Cells[startIndex + skuStartIndex, 41, startIndex + skuEndIndex, 41])
                    //            {
                    //                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                    //                rng.Merge = true;
                    //                rng.Value = it.MSKU;
                    //            }
                    //        }
                    //        catch(Exception err)
                    //        {

                    //        }

                    //    }

                    //    lastMSKU = it.MSKU;
                    //}
                    if (ssdx == data.Items.Count - 1)
                    {
                        if (rowIndex - startIndex > 0)
                        {
                            using (var rng = sheet.Cells[startIndex, 35, rowIndex, 35])
                            {
                                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                                rng.Merge = true;
                            }

                            //using (var rng = sheet.Cells[startIndex, 28, rowIndex, 28])
                            //{
                            //    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                            //    rng.Merge = true;
                            //}
                            //using (var rng = sheet.Cells[startIndex, 29, rowIndex, 29])
                            //{
                            //    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                            //    rng.Merge = true;
                            //}
                        }
                    }
                    rowIndex++;

                }

                if (idx == list数据透视.Count - 1)
                {
                    using (var rng = sheet.Cells[1, 35, rowIndex - 1, 39])
                    {
                        rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    }

                    //using (var rng = sheet.Cells[1, 24, 1, 28])
                    //{
                    //    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                    //    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                    //}
                    sheet.Column(35).Width = 30;
                    sheet.Column(36).Width = 22;
                    sheet.Column(37).Width = 18;
                    sheet.Column(38).Width = 20;
                    sheet.Column(39).Width = 10;
                    sheet.Column(39).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }

            // 透视信息
            sheet.Cells[1, 24].Value = "店铺";
            sheet.Cells[1, 25].Value = "MSKU";
            sheet.Cells[1, 26].Value = "赔偿编号";
            sheet.Cells[1, 27].Value = "数量";
            sheet.Cells[1, 28].Value = "总数量";
            for (int idx = 0, rowIndex = 2; idx < list数据透视.Count; idx++)
            {
                var data = list数据透视[idx];
                var startIndex = rowIndex;

                var skus = data.Items.Select(x => x.MSKU).Distinct().ToList();
                decimal totalSum = 0;
                for (var ssdx = 0; ssdx < skus.Count; ssdx++)
                {
                    var sku = skus[ssdx];
                    var ss = data.Items.Where(x => x.MSKU == sku)
                        .GroupBy(x => x._赔偿编号)
                         .Select(g => g.First())
                         .ToList();
                    sheet.Cells[rowIndex, 25].Value = sku;
                    sheet.Cells[rowIndex, 26].Value = string.Join(",", ss.Select(x => x._赔偿编号).ToList());
                    var s = ss.Select(x => x._数量).Sum();
                    sheet.Cells[rowIndex, 27].Value = s;
                    totalSum += s;
                    if (ssdx == skus.Count - 1)
                    {
                        sheet.Cells[startIndex, 24].Value = data._店铺;
                        sheet.Cells[startIndex, 28].Value = totalSum;
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
                        }

                    }
                    rowIndex++;

                }


                if (idx == list数据透视.Count - 1)
                {
                    using (var rng = sheet.Cells[1, 24, rowIndex - 1, 28])
                    {
                        rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    }

                    using (var rng = sheet.Cells[1, 24, 1, 28])
                    {
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                        rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                    }
                    sheet.Column(24).Width = 30;
                    sheet.Column(25).Width = 22;
                    sheet.Column(26).Width = 30;
                    sheet.Column(27).Width = 10;
                    sheet.Column(28).Width = 10;
                    //sheet.Column(29).Width = 10;
                    //sheet.Column(26).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Column(27).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Column(28).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //sheet.Column(29).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }
        }
    }

    class _赔偿订单数据透视
    {
        public string _店铺 { get; set; }
        public List<_赔偿订单统计项> Items { get; set; } = new List<_赔偿订单统计项>();
        //public decimal _总数量合计 { get { return Items.Select(x => x._数量).Sum(); } }
        //public decimal _总金额合计 { get { return Items.Select(x => x._金额).Sum(); } }
    }

    class _赔偿订单统计项
    {
        public string MSKU { get; set; }
        public string _国家 { get; set; }
        public string _赔偿编号 { get; set; }
        public decimal _数量 { get; set; }
        public decimal _金额 { get; set; }
    }

    class _退货单数据透视
    {
        public string _店铺 { get; set; }
        public List<_退货单数据统计项> Items { get; set; } = new List<_退货单数据统计项>();
        public decimal _总数量合计 { get { return Items.Select(x => x._数量).Sum(); } }
        public decimal _总金额合计 { get { return Items.Select(x => x._金额).Sum(); } }
    }

    class _退货单数据统计项
    {
        public string MSKU { get; set; }
        public decimal _数量 { get; set; }
        public decimal _金额 { get; set; }
    }
}
