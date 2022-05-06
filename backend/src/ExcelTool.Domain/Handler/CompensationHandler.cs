using ExcelTool.Domain.Models.Commons;
using ExcelTool.Domain.Models.Compensations;
using ExcelTool.Domain.Utils;
using Npoi.Mapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExcelTool.Domain.Handler.Compensations
{
    public class CompensationHandler
    {
        protected string path赔偿订单文件;
        protected string path退货订单文件;
        protected string path处理方案文件;
        protected string path部门映射文件;
        protected string path店铺更名匹配文件;
        protected string pathMSKU2SKU映射文件;
        protected string pathSKU2单价映射文件;
        protected string folder临时文件夹;

        #region ctor
        protected CompensationHandler()
        {

        }
        public CompensationHandler(string p赔偿订单文件, string p退货订单文件, string p处理方案文件, string p部门映射文件, string p店铺更名匹配文件, string pMSKU2SKU映射文件, string pSKU2单价映射文件, string f临时文件夹)
            : this()
        {
            path赔偿订单文件 = p赔偿订单文件;
            path退货订单文件 = p退货订单文件;
            path处理方案文件 = p处理方案文件;
            path部门映射文件 = p部门映射文件;
            path店铺更名匹配文件 = p店铺更名匹配文件;
            pathMSKU2SKU映射文件 = pMSKU2SKU映射文件;
            pathSKU2单价映射文件 = pSKU2单价映射文件;
            folder临时文件夹 = f临时文件夹;
        }
        #endregion

        public async Task<MemoryStream> Handle()
        {
            var list退货订单 = new List<_退货订单>();
            var list赔偿订单 = new List<_赔偿订单>();
            var list退货处理方案 = new List<_赔偿退货处理方案>();
            var list赔偿处理方案 = new List<_赔偿退货处理方案>();
            var list站点更名匹配 = new List<_站点更名匹配表>();
            var listSKU匹配 = new List<_SKU匹配>();
            var list价格匹配 = new List<_价格匹配>();
            var list部门匹配 = new List<_部门匹配>();
            var dict赔偿订单唯一值to国家 = new Dictionary<string, string>();
            var dict赔偿订单统计项映射 = new Dictionary<string, _赔偿订单统计项>();
            var dict退货订单唯一值to国家 = new Dictionary<string, string>();
            var dict退货订单统计项映射 = new Dictionary<string, _退货订单统计项>();

            var util店铺更名工具 = new StoreNameHelper();
            var util赔偿处理方案匹配 = new _赔偿退货处理方案匹配Util();
            var util退货处理方案匹配 = new _赔偿退货处理方案匹配Util();
            var utilSKU成本价匹配 = new _SKU成本单价匹配Util();
            var util部门匹配 = new _部门匹配Util();
            var list赔偿订单统计项 = new List<_赔偿订单统计项>();
            var list退货订单统计项 = new List<_退货订单统计项>();
            //var list部门 = new List<string>();

            #region 数据读取
            if (!string.IsNullOrEmpty(path店铺更名匹配文件))
            {
                var mapper = new Mapper(path店铺更名匹配文件);
                var sheetDatas = mapper.Take<_站点更名匹配表>().Select(x => x.Value).ToList();
                sheetDatas.ForEach(it =>
                {
                    if (it._领星店铺名称 != it._南棠店铺名称)
                    {
                        list站点更名匹配.Add(it);
                    }
                });
                util店铺更名工具.LoadData(list站点更名匹配);
            }

            if (!string.IsNullOrEmpty(pathSKU2单价映射文件))
            {
                var mapper = new Mapper(pathSKU2单价映射文件);
                var sheetDatas = mapper.Take<_价格匹配>().Select(x => x.Value).ToList();
                list价格匹配.AddRange(sheetDatas);
            }

            if (!string.IsNullOrEmpty(pathMSKU2SKU映射文件))
            {
                var mapper = new Mapper(pathMSKU2SKU映射文件);
                var sheetDatas = mapper.Take<_SKU匹配>().Select(x => x.Value).ToList();
                listSKU匹配.AddRange(sheetDatas);
            }

            if (!string.IsNullOrEmpty(path赔偿订单文件))
            {
                var mapper = new Mapper(path赔偿订单文件);
                var sheetDatas = mapper.Take<_赔偿订单>().Select(x => x.Value).ToList();
                sheetDatas.ForEach(it =>
                {
                    it._南棠店铺名称 = util店铺更名工具._标准化店铺名称(it._国家, it._领星店铺名称);
                    it._数据处理();
                    list赔偿订单.Add(it);
                });
            }

            if (!string.IsNullOrEmpty(path退货订单文件))
            {
                var mapper = new Mapper(path退货订单文件);
                var sheetDatas = mapper.Take<_退货订单>().Select(x => x.Value).ToList();
                sheetDatas.ForEach(it =>
                {
                    it._南棠店铺名称 = util店铺更名工具._标准化店铺名称(it._国家, it._领星店铺名称);
                    it._数据处理();
                    list退货订单.Add(it);
                });
            }

            if (!string.IsNullOrEmpty(path处理方案文件))
            {
                var mapper = new Mapper(path处理方案文件);
                var data1 = mapper.Take<_赔偿退货处理方案>("赔偿处理方案").Select(x => x.Value).ToList();
                list赔偿处理方案.AddRange(data1);

                var data2 = mapper.Take<_赔偿退货处理方案>("退货处理方案").Select(x => x.Value).ToList();
                list退货处理方案.AddRange(data2);
            }

            if (!string.IsNullOrEmpty(path部门映射文件))
            {
                var mapper = new Mapper(path部门映射文件);
                var sheetDatas = mapper.Take<_部门匹配>().Select(x => x.Value).ToList();
                sheetDatas.ForEach(it =>
                {
                    string pattern = @"[一二三四五六七八九十]{1,2}部";
                    var m = Regex.Match(it._负责人部门, pattern);
                    if (m.Success)
                    {
                        //list部门.Add(m.Value);
                        it._部门 = m.Value;
                    }
                    else
                    {
                        it._部门 = "未匹配";
                        //list部门.Add("未匹配");
                    }
                });

                list部门匹配.AddRange(sheetDatas);
                // 添加未匹配部门
                //list部门.Add("未匹配");
                //list部门 = list部门.Select(n => n).Distinct().ToList();
                util部门匹配.loadData(list部门匹配);
            }
            #endregion

            #region 数据处理
            var list赔偿单需要EPR处理的 = list赔偿处理方案.Where(x => x._需要ERP操作).ToList();
            var list退货单需要EPR处理的 = list退货处理方案.Where(x => x._需要ERP操作).ToList();

            util赔偿处理方案匹配.loadData(list赔偿处理方案);
            util退货处理方案匹配.loadData(list退货单需要EPR处理的);
            utilSKU成本价匹配.loadData(listSKU匹配, list价格匹配);

            #region 处理赔偿订单数据
            list赔偿订单.ForEach(it =>
            {
                if (dict赔偿订单唯一值to国家.ContainsKey(it.Key))
                {
                    // 判断国家是不是一个,不是的话 只取一个国家作为计算
                    if (it._国家 == dict赔偿订单唯一值to国家[it.Key])
                    {
                        var ditem = dict赔偿订单统计项映射[it.Key];
                        ditem._合并汇总(util赔偿处理方案匹配._匹配操作(it), it);
                    }
                }
                else
                {
                    var ditem = _赔偿订单统计项.From(util赔偿处理方案匹配._匹配操作(it), it);
                    util部门匹配._匹配部门(ditem);
                    utilSKU成本价匹配.匹配数据(ditem);
                    dict赔偿订单统计项映射[it.Key] = ditem;
                    dict赔偿订单唯一值to国家[it.Key] = it._国家;
                    list赔偿订单统计项.Add(ditem);
                }
            });
            #endregion

            #region 处理退货订单数据
            list退货订单.ForEach(it =>
            {
                if (dict退货订单唯一值to国家.ContainsKey(it.Key))
                {
                    // 判断国家是不是一个,不是的话 只取一个国家作为计算
                    if (it._国家 == dict退货订单唯一值to国家[it.Key])
                    {
                        var ditem = dict退货订单统计项映射[it.Key];
                        ditem._合并汇总(util退货处理方案匹配._匹配操作(it), it);
                    }
                }
                else
                {
                    var ditem = _退货订单统计项.From(util退货处理方案匹配._匹配操作(it), it);
                    util部门匹配._匹配部门(ditem);
                    utilSKU成本价匹配.匹配数据(ditem);
                    dict退货订单统计项映射[it.Key] = ditem;
                    dict退货订单唯一值to国家[it.Key] = it._国家;
                    list退货订单统计项.Add(ditem);
                }
            });
            #endregion

            #endregion

            var exportFilePath = Path.Combine(folder临时文件夹, "result.xlsx");

            #region 导出数据
            using (var package = new ExcelPackage(new FileInfo(exportFilePath)))
            {
                var workbox = package.Workbook;

                #region 亚马逊赔偿ERP需要做可售库存弃置处理
                {
                    var datas = list赔偿订单统计项.Where(x => x._ERP执行动作 == "可售库存弃置").ToList();
                    if (datas.Count > 0)
                    {
                        var sheet = workbox.Worksheets.Add($"亚马逊赔偿ERP需要做可售库存弃置处理");
                        _生成赔偿订单Sheet(sheet, datas);
                    }
                }
                #endregion

                #region 亚马逊赔偿ERP需要做可售库存带成本入库处理
                {
                    var datas = list赔偿订单统计项.Where(x => x._ERP执行动作 == "做可售库存带成本入库").ToList();
                    if (datas.Count > 0)
                    {
                        var sheet = workbox.Worksheets.Add($"亚马逊赔偿ERP需要做可售库存带成本入库处理");
                        _生成赔偿订单Sheet(sheet, datas);
                    }
                }
                #endregion

                #region 亚马逊退货订单ERP需要做可售库存0成本入库处理
                {
                    var datas = list退货订单统计项.Where(x => x._ERP执行动作 == "0成本入库").ToList();
                    if (datas.Count > 0)
                    {
                        var sheet = workbox.Worksheets.Add($"亚马逊退货订单ERP需要做可售库存0成本入库处理");
                        _生成退货订单Sheet(sheet, datas);
                    }
                }
                #endregion

                #region 赔偿订单统计详情
                {
                    if (list赔偿订单统计项.Count > 0)
                    {
                        var sheet = workbox.Worksheets.Add($"赔偿订单统计详情");
                        _生成赔偿订单详情Sheet(sheet, list赔偿订单统计项);
                    }
                }
                #endregion

                #region 退货订单统计详情
                {
                    if (list退货订单统计项.Count > 0)
                    {
                        var sheet = workbox.Worksheets.Add($"退货订单统计详情");
                        _生成退货订单详情Sheet(sheet, list退货订单统计项);
                    }
                }
                #endregion
                package.Save();
            }
            #endregion

            var aa = 1;

            var memoryStream = new MemoryStream();
            using (var fs = File.OpenRead(exportFilePath))
                fs.CopyTo(memoryStream);
            File.Delete(exportFilePath);
            return memoryStream;
        }

        private static void _生成赔偿订单Sheet(ExcelWorksheet sheet, List<_赔偿订单统计项> datas)
        {
            #region 标题列
            sheet.Cells[2, 1].Value = "统计编号";
            sheet.Cells[2, 2].Value = "统计个数";
            sheet.Cells[2, 3].Value = "领星店铺";
            sheet.Cells[2, 4].Value = "南棠店铺";
            sheet.Cells[2, 5].Value = "MSKU";
            sheet.Cells[2, 6].Value = "SKU";
            sheet.Cells[2, 7].Value = "赔偿编号";
            sheet.Cells[2, 8].Value = "数量";
            sheet.Cells[2, 9].Value = "采购成本单价";
            sheet.Cells[2, 10].Value = "弃置单号";
            sheet.Cells[2, 11].Value = "审批单号";
            sheet.Cells[2, 12].Value = "备注";
            sheet.Cells[2, 13].Value = "部门";
            #endregion

            #region 数据列
            var startRow = 3;
            var rowIndex = startRow;

            for (int idx = 0; idx < datas.Count; idx++)
            {
                var data = datas[idx];
                sheet.Cells[rowIndex, 1].Value = data.UniqCode;
                sheet.Cells[rowIndex, 2].Value = data.Items.Count;
                sheet.Cells[rowIndex, 3].Value = data._领星店铺名称;
                sheet.Cells[rowIndex, 4].Value = data._南棠店铺名称;
                sheet.Cells[rowIndex, 5].Value = data.MSKU;
                sheet.Cells[rowIndex, 6].Value = data.SKU;
                sheet.Cells[rowIndex, 7].Value = data._赔偿编号;
                sheet.Cells[rowIndex, 8].Value = data._ERP操作数量;
                sheet.Cells[rowIndex, 9].Value = data._采购成本单价;

                sheet.Cells[rowIndex, 13].Value = data._部门;
                rowIndex++;
            }
            #endregion

            #region 表格整体配置
            sheet.Column(1).Width = 12;
            sheet.Column(2).Width = 10;
            sheet.Column(3).Width = 30;
            sheet.Column(4).Width = 30;
            sheet.Column(5).Width = 30;
            sheet.Column(6).Width = 18;
            sheet.Column(7).Width = 16;
            sheet.Column(8).Width = 12;
            sheet.Column(9).Width = 14;
            sheet.Column(10).Width = 16;
            sheet.Column(11).Width = 16;
            sheet.Column(12).Width = 16;
            sheet.Column(13).Width = 14;
            sheet.Row(1).Height = 46;

            using (var rng = sheet.Cells[1, 1, 1, 13])
            {
                rng.Merge = true;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                rng.RichText.Add($"{sheet.Name}\r\n");
                rng.RichText.Add($"领星ERP导出时间:*年*月*号-*年*月*号");
                rng.Style.WrapText = true;
                rng.Style.Font.Size = 14;
            }

            using (var rng = sheet.Cells[1, 1, rowIndex - 1, 13])
            {
                rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            }

            using (var rng = sheet.Cells[2, 1, 2, 13])
            {
                var colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
            }
            #endregion
        }

        private static void _生成退货订单Sheet(ExcelWorksheet sheet, List<_退货订单统计项> datas)
        {
            #region 标题列
            sheet.Cells[2, 1].Value = "统计编号";
            sheet.Cells[2, 2].Value = "统计个数";
            sheet.Cells[2, 3].Value = "领星店铺";
            sheet.Cells[2, 4].Value = "南棠店铺";
            sheet.Cells[2, 5].Value = "MSKU";
            sheet.Cells[2, 6].Value = "SKU";
            sheet.Cells[2, 7].Value = "数量";
            sheet.Cells[2, 8].Value = "采购成本单价";
            sheet.Cells[2, 9].Value = "弃置单号";
            sheet.Cells[2, 10].Value = "审批单号";
            sheet.Cells[2, 11].Value = "备注";
            sheet.Cells[2, 12].Value = "部门";
            #endregion

            #region 数据列
            var startRow = 3;
            var rowIndex = startRow;

            for (int idx = 0; idx < datas.Count; idx++)
            {
                var data = datas[idx];
                sheet.Cells[rowIndex, 1].Value = data.UniqCode;
                sheet.Cells[rowIndex, 2].Value = data.Items.Count;
                sheet.Cells[rowIndex, 3].Value = data._领星店铺名称;
                sheet.Cells[rowIndex, 4].Value = data._南棠店铺名称;
                sheet.Cells[rowIndex, 5].Value = data.MSKU;
                sheet.Cells[rowIndex, 6].Value = data.SKU;
                sheet.Cells[rowIndex, 7].Value = data._ERP操作数量;
                sheet.Cells[rowIndex, 8].Value = data._采购成本单价;
                sheet.Cells[rowIndex, 12].Value = data._部门;
                rowIndex++;
            }
            #endregion

            #region 表格整体配置
            sheet.Column(1).Width = 12;
            sheet.Column(2).Width = 10;
            sheet.Column(3).Width = 30;
            sheet.Column(4).Width = 30;
            sheet.Column(5).Width = 30;
            sheet.Column(6).Width = 18;
            sheet.Column(7).Width = 12;
            sheet.Column(8).Width = 14;
            sheet.Column(9).Width = 16;
            sheet.Column(10).Width = 16;
            sheet.Column(11).Width = 16;
            sheet.Column(12).Width = 14;
            sheet.Row(1).Height = 46;

            using (var rng = sheet.Cells[1, 1, 1, 12])
            {
                rng.Merge = true;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                rng.RichText.Add($"{sheet.Name}\r\n");
                rng.RichText.Add($"领星ERP导出时间:*年*月*号-*年*月*号");
                rng.Style.WrapText = true;
                rng.Style.Font.Size = 14;
            }

            using (var rng = sheet.Cells[1, 1, rowIndex - 1, 12])
            {
                rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            }

            using (var rng = sheet.Cells[2, 1, 2, 12])
            {
                var colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
            }
            #endregion
        }

        private static void _生成赔偿订单详情Sheet(ExcelWorksheet sheet, List<_赔偿订单统计项> datas)
        {
            #region 标题行
            sheet.Cells[1, 1].Value = "统计编号";
            sheet.Cells[1, 2].Value = "店铺";
            sheet.Cells[1, 3].Value = "国家";
            sheet.Cells[1, 4].Value = "时间";
            sheet.Cells[1, 5].Value = "赔偿编号";
            sheet.Cells[1, 6].Value = "订单号";
            sheet.Cells[1, 7].Value = "原因";
            sheet.Cells[1, 8].Value = "MSKU";
            sheet.Cells[1, 9].Value = "FNSKU";
            sheet.Cells[1, 10].Value = "ASIN";
            sheet.Cells[1, 11].Value = "标题";
            sheet.Cells[1, 12].Value = "状况";
            sheet.Cells[1, 13].Value = "币种";
            sheet.Cells[1, 14].Value = "每件商品赔偿金额";
            sheet.Cells[1, 15].Value = "总金额";
            sheet.Cells[1, 16].Value = "赔偿数量（现金）";
            sheet.Cells[1, 17].Value = "赔偿数量（库存）";
            sheet.Cells[1, 18].Value = "赔偿数量（总计）";
            sheet.Cells[1, 19].Value = "原始赔偿编号";
            sheet.Cells[1, 20].Value = "原始赔偿类型";
            sheet.Cells[1, 21].Value = "部门";
            #endregion

            #region 数据列
            var startRow = 2;
            var rowIndex = startRow;

            for (int idx = 0; idx < datas.Count; idx++)
            {
                var data = datas[idx];
                data.Items.ForEach(it =>
                {
                    sheet.Cells[rowIndex, 1].Value = data.UniqCode;
                    sheet.Cells[rowIndex, 2].Value = it._领星店铺名称;
                    sheet.Cells[rowIndex, 3].Value = it._国家;
                    sheet.Cells[rowIndex, 4].Value = it._时间;
                    sheet.Cells[rowIndex, 5].Value = it._赔偿编号;
                    sheet.Cells[rowIndex, 6].Value = it._订单号;
                    sheet.Cells[rowIndex, 7].Value = it._原因;
                    sheet.Cells[rowIndex, 8].Value = it.MSKU;
                    sheet.Cells[rowIndex, 9].Value = it.FNSKU;
                    sheet.Cells[rowIndex, 10].Value = it.ASIN;
                    sheet.Cells[rowIndex, 11].Value = it._标题;
                    sheet.Cells[rowIndex, 12].Value = it._状况;
                    sheet.Cells[rowIndex, 13].Value = it._币种;
                    sheet.Cells[rowIndex, 14].Value = it._每件商品赔偿金额;
                    sheet.Cells[rowIndex, 15].Value = it._总金额;
                    sheet.Cells[rowIndex, 16].Value = it._赔偿数量_现金;
                    sheet.Cells[rowIndex, 17].Value = it._赔偿数量_库存;
                    sheet.Cells[rowIndex, 18].Value = it._赔偿数量_总计;
                    sheet.Cells[rowIndex, 19].Value = it._原始赔偿编号;
                    sheet.Cells[rowIndex, 20].Value = it._原始赔偿类型;
                    sheet.Cells[rowIndex, 21].Value = data._部门;
                    rowIndex++;
                });

            }
            #endregion
        }

        private static void _生成退货订单详情Sheet(ExcelWorksheet sheet, List<_退货订单统计项> datas)
        {
            #region 标题行
            sheet.Cells[1, 1].Value = "统计编号";
            sheet.Cells[1, 2].Value = "店铺";
            sheet.Cells[1, 3].Value = "国家";
            sheet.Cells[1, 4].Value = "品名";
            sheet.Cells[1, 5].Value = "sku";
            sheet.Cells[1, 6].Value = "分类";
            sheet.Cells[1, 7].Value = "品牌";
            sheet.Cells[1, 8].Value = "订单号";
            sheet.Cells[1, 9].Value = "商品名称";
            sheet.Cells[1, 10].Value = "MSKU";
            sheet.Cells[1, 11].Value = "ASIN";
            sheet.Cells[1, 12].Value = "数量";
            sheet.Cells[1, 13].Value = "发货仓库编号";
            sheet.Cells[1, 14].Value = "库存属性";
            sheet.Cells[1, 15].Value = "退货原因";
            sheet.Cells[1, 16].Value = "状态";
            sheet.Cells[1, 17].Value = "LPN编号";
            sheet.Cells[1, 18].Value = "买家备注";
            sheet.Cells[1, 19].Value = "退货时间";
            sheet.Cells[1, 20].Value = "订购时间";
            sheet.Cells[1, 21].Value = "备注";
            sheet.Cells[1, 22].Value = "部门";
            #endregion

            #region 数据列
            var startRow = 2;
            var rowIndex = startRow;

            for (int idx = 0; idx < datas.Count; idx++)
            {
                var data = datas[idx];
                data.Items.ForEach(it =>
                {
                    sheet.Cells[rowIndex, 1].Value = data.UniqCode;
                    sheet.Cells[rowIndex, 2].Value = it._领星店铺名称;
                    sheet.Cells[rowIndex, 3].Value = it._国家;
                    sheet.Cells[rowIndex, 4].Value = it._品名;
                    sheet.Cells[rowIndex, 5].Value = it.SKU;
                    sheet.Cells[rowIndex, 6].Value = it._分类;
                    sheet.Cells[rowIndex, 7].Value = it._品牌;
                    sheet.Cells[rowIndex, 8].Value = it._订单号;
                    sheet.Cells[rowIndex, 9].Value = it._商品名称;
                    sheet.Cells[rowIndex, 10].Value = it.MSKU;
                    sheet.Cells[rowIndex, 11].Value = it.ASIN;
                    sheet.Cells[rowIndex, 12].Value = it._数量;
                    sheet.Cells[rowIndex, 13].Value = it._发货仓库编号;
                    sheet.Cells[rowIndex, 14].Value = it._原因;
                    sheet.Cells[rowIndex, 15].Value = it._退货原因;
                    sheet.Cells[rowIndex, 16].Value = it._状态;
                    sheet.Cells[rowIndex, 17].Value = it.LPN编号;
                    sheet.Cells[rowIndex, 18].Value = it._买家备注;
                    sheet.Cells[rowIndex, 19].Value = it._退货时间;
                    sheet.Cells[rowIndex, 20].Value = it._订购时间;
                    sheet.Cells[rowIndex, 21].Value = it._备注;
                    sheet.Cells[rowIndex, 22].Value = data._部门;
                    rowIndex++;
                });

            }
            #endregion
        }
    }

    enum enum处理平台
    {
        _ERP,
        _后台
    }

    static class Action赔偿所有动作
    {
        public static readonly string _可售库存弃置 = "可售库存弃置";
        public static readonly string _不做任何操作 = "不做任何操作";
        public static readonly string _做可售库存带成本入库 = "做可售库存带成本入库";
    }

    class 赔偿退货统计项
    {
        public string _领星店铺名称 { get; set; }
        public string _南棠店铺名称 { get; set; }
        public string MSKU { get; set; }
        public string SKU { get; set; }
        public decimal _采购成本单价 { get; set; }
        public int _ERP操作数量 { get; set; }
        public int _后台操作数量 { get; set; }
        public _赔偿退货处理方案 _处理方案 { get; set; }
        public bool _需要ERP操作
        {
            get
            {
                if (_处理方案 == null)
                { return false; }
                return _处理方案._需要ERP操作;
            }
        }
        public bool _需要后台操作
        {
            get
            {
                if (_处理方案 == null)
                { return false; }
                return _处理方案._需要后台操作;
            }
        }
        public string _ERP执行动作
        {
            get
            {
                if (_处理方案 == null)
                { return null; }
                return _处理方案._ERP执行动作;
            }
        }
        public string _后台执行动作
        {
            get
            {
                if (_处理方案 == null)
                { return null; }
                return _处理方案._后台执行动作;
            }
        }
        public string _原因 { get; protected set; }
        public string _部门 { get; set; }
        public string UniqCode { get; protected set; }

        protected 赔偿退货统计项()
        {
            UniqCode = Guid.NewGuid().ToString("N").ToUpper();
        }
    }

    class _赔偿订单统计项 : 赔偿退货统计项
    {
        public string _赔偿编号 { get; set; }
        public List<_赔偿订单> Items { get; protected set; } = new List<_赔偿订单>();

        public _赔偿订单统计项()
            : base()
        { }

        public void _合并汇总(_赔偿退货处理方案 solution, _赔偿订单 data)
        {
            _ERP操作数量 += _判断获取数量(solution, data, enum处理平台._ERP);
            _后台操作数量 += _判断获取数量(solution, data, enum处理平台._后台);
            Items.Add(data);
        }

        public static _赔偿订单统计项 From(_赔偿退货处理方案 solution, _赔偿订单 data)
        {
            var item = new _赔偿订单统计项();
            item._领星店铺名称 = data._领星店铺名称;
            item._南棠店铺名称 = data._南棠店铺名称;
            item.MSKU = data.MSKU;
            item._赔偿编号 = data._赔偿编号;
            item._处理方案 = solution;
            item._原因 = data._原因;
            item._ERP操作数量 = _判断获取数量(solution, data, enum处理平台._ERP);
            item._后台操作数量 = _判断获取数量(solution, data, enum处理平台._后台);
            item.Items.Add(data);
            return item;
        }

        protected static int _判断获取数量(_赔偿退货处理方案 solution, _赔偿订单 data, enum处理平台 _处理平台)
        {
            int amount = 0;
            if (solution == null)
            { return amount; }
            string action = _处理平台 == enum处理平台._ERP ? solution._ERP执行动作 : solution._后台执行动作;
            switch (action)
            {
                case "可售库存弃置":
                    amount = data._赔偿数量_总计;
                    break;
                case "做可售库存带成本入库":
                    amount = data._赔偿数量_库存;
                    break;
                default:
                    break;
            }
            return amount;
        }
    }

    class _退货订单统计项 : 赔偿退货统计项
    {
        public List<_退货订单> Items { get; protected set; } = new List<_退货订单>();

        public _退货订单统计项()
            : base()
        { }

        public void _合并汇总(_赔偿退货处理方案 solution, _退货订单 data)
        {
            _ERP操作数量 += _判断获取数量(solution, data, enum处理平台._ERP);
            _后台操作数量 += _判断获取数量(solution, data, enum处理平台._后台);
            Items.Add(data);
        }

        public static _退货订单统计项 From(_赔偿退货处理方案 solution, _退货订单 data)
        {
            var item = new _退货订单统计项();
            item._领星店铺名称 = data._领星店铺名称;
            item._南棠店铺名称 = data._南棠店铺名称;
            item.MSKU = data.MSKU;
            item._处理方案 = solution;
            item._原因 = data._原因;
            item._ERP操作数量 = _判断获取数量(solution, data, enum处理平台._ERP);
            item._后台操作数量 = _判断获取数量(solution, data, enum处理平台._后台);
            item.Items.Add(data);
            return item;
        }

        protected static int _判断获取数量(_赔偿退货处理方案 solution, _退货订单 data, enum处理平台 _处理平台)
        {
            int amount = 0;
            if (solution == null)
            { return amount; }
            string action = _处理平台 == enum处理平台._ERP ? solution._ERP执行动作 : solution._后台执行动作;
            switch (action)
            {
                case "0成本入库":
                    amount = data._数量;
                    break;
                default:
                    break;
            }
            return amount;
        }
    }

    class _赔偿退货处理方案匹配Util
    {
        protected Dictionary<string, _赔偿退货处理方案> Maps = new Dictionary<string, _赔偿退货处理方案>();

        public void loadData(List<_赔偿退货处理方案> list)
        {
            list.ForEach(it =>
            {
                Maps.Add(it.reason, it);
            });
        }

        public _赔偿退货处理方案 _匹配操作(赔偿退货订单源数据 data)
        {
            if (string.IsNullOrEmpty(data._原因))
            { return null; }
            if (!Maps.ContainsKey(data._原因))
            { return null; }
            return Maps[data._原因];
        }
    }

    class _SKU成本单价匹配Util
    {
        protected Dictionary<string, string> dictMSKU2SKU映射 = new Dictionary<string, string>();
        protected Dictionary<string, decimal> dictSKU成本价映射 = new Dictionary<string, decimal>();

        public void loadData(List<_SKU匹配> listSKU匹配, List<_价格匹配> list价格匹配)
        {
            listSKU匹配.ForEach(it =>
            {
                dictMSKU2SKU映射[it.MSKU] = it.SKU;
            });
            list价格匹配.ForEach(it =>
            {
                dictSKU成本价映射[it.SKU] = it._成本单价;
            });
        }

        public void 匹配数据(赔偿退货统计项 data)
        {
            if (!string.IsNullOrWhiteSpace(data.MSKU) && dictMSKU2SKU映射.ContainsKey(data.MSKU))
            {
                data.SKU = dictMSKU2SKU映射[data.MSKU];
                if (!string.IsNullOrWhiteSpace(data.SKU) && dictSKU成本价映射.ContainsKey(data.SKU))
                {
                    data._采购成本单价 = dictSKU成本价映射[data.SKU];
                }
            }
        }
    }

    class _部门匹配Util
    {
        protected Dictionary<string, string> Maps = new Dictionary<string, string>();

        public void loadData(List<_部门匹配> list)
        {
            list.ForEach(it =>
            {
                Maps.Add(it._店铺名, it._部门);
            });
        }

        public void _匹配部门(赔偿退货统计项 data)
        {
            var dep = "未匹配";
            if (!string.IsNullOrWhiteSpace(data._南棠店铺名称) && Maps.ContainsKey(data._南棠店铺名称))
            {
                dep = Maps[data._南棠店铺名称];
            }
            data._部门 = dep;
        }
    }
}
