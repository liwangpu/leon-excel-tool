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

namespace ExcelTool.Commands.StockAnalysiss
{

    public class StockAnalysisUploadCommandHandler : IRequestHandler<StockAnalysisUploadCommand>
    {

        private readonly IStaticFileSetting fileSetting;

        public StockAnalysisUploadCommandHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }


        public async Task<Unit> Handle(StockAnalysisUploadCommand request, CancellationToken cancellationToken)
        {
            var currentTmpFolder = fileSetting.GenerateTemporaryFolder();

            var list总账明细 = new List<_库存出入总账>();
            var listSKU归属映射 = new List<_SKU归属>();
            //var list销售流水 = new List<_销售流水>();
            var list数据透视 = new List<_总账数据透视1>();
            var list弃置明细 = new List<_库存出入总账>();
            var dict销售流水 = new Dictionary<string, _销售流水>();

            #region 读取总账明细信息
            if (request._总账明细 != null)
            {
                var daysMap = new Dictionary<string, int>();
                var detailFilePaths = request._总账明细.Select(f =>
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


                if (detailFilePaths.Count > 0)
                {
                    for (var idx = 0; idx < detailFilePaths.Count; idx++)
                    {
                        var filePath = detailFilePaths[idx];
                        using (var targetStream = File.Create(filePath))
                        {
                            await request._总账明细[idx].CopyToAsync(targetStream);
                            targetStream.Close();
                        }
                    }
                    var dict = new Dictionary<string, _总账数据透视1>();
                    for (var idx = 0; idx < detailFilePaths.Count; idx++)
                    {
                        var filePath = detailFilePaths[idx];
                        int day = daysMap[filePath];
                        var mapper = new Mapper(filePath);
                        var sheetDatas = mapper.Take<_库存出入总账>("Worksheet").Select(x => x.Value).ToList();
                        sheetDatas.ForEach(it =>
                        {
                            it._天数 = day;
                            it.InitData();
                            if (it._退换或弃置)
                            {
                                list弃置明细.Add(it);
                                return;
                            }
                            var item = dict.ContainsKey(it._仓库店铺SKU) ? dict[it._仓库店铺SKU] : new _总账数据透视1() { _仓库店铺SKU = it._仓库店铺SKU, _仓库 = it._仓库, _店铺 = it._店铺, SKU = it.SKU };
                            if (item._仓库类型 == null)
                            {
                                it._校验仓库类型();
                                item._仓库类型 = it._仓库类型;
                            }
                            item.Items.Add(it);
                            dict[it._仓库店铺SKU] = item;

                        });
                    }

                    foreach (var v in dict.Values)
                    {
                        list数据透视.Add(v);
                    }
                }
            }
            #endregion

            #region 读取SKU归属映射
            if (request.SKU匹配文件 != null)
            {
                //for (var idx = request.SKUMappingFiles.Count - 1; idx >= 0; idx--)
                //{
                //    var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString()}.xlsx");
                //    using (var targetStream = File.Create(filePath))
                //    {
                //        await request.SKUMappingFiles[idx].CopyToAsync(targetStream);
                //        targetStream.Close();
                //        using (var package = new ExcelPackage(new FileInfo(filePath)))
                //        {
                //            for (var sdx = package.Workbook.Worksheets.Count - 1; sdx >= 0; sdx--)
                //            {
                //                var sheet = package.Workbook.Worksheets[sdx];
                //                var sheetDatas = SheetReader<_SKU归属>.From(sheet);
                //                listSKU归属映射.AddRange(sheetDatas);
                //            }
                //        }
                //    }
                //}
            }
            #endregion

            #region 读取销售流水
            if (request._销售流水 != null)
            {
                for (var idx = request._销售流水.Count - 1; idx >= 0; idx--)
                {
                    var filePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");
                    using (var targetStream = File.Create(filePath))
                    {
                        await request._销售流水[idx].CopyToAsync(targetStream);
                        targetStream.Close();
                        var mapper = new Mapper(filePath);
                        var sheetDatas = mapper.Take<_销售流水>().Select(x => x.Value).ToList();
                        sheetDatas.ForEach(it =>
                        {
                            var item = dict销售流水.ContainsKey(it._仓库店铺SKU) ? dict销售流水[it._仓库店铺SKU] : new _销售流水() { _仓库店铺SKU = it._仓库店铺SKU };
                            item._销售金额 += it._销售金额;
                            dict销售流水[it._仓库店铺SKU] = item;
                        });
                    }
                }
            }
            #endregion

            #region 数据处理

            list数据透视.ForEach(it =>
            {
                it._计算几个数量金额信息();
                if (dict销售流水.ContainsKey(it._仓库店铺SKU))
                {
                    var v = dict销售流水[it._仓库店铺SKU];
                    v._计算平均销售金额();

                    it._销售金额 = v._销售金额;
                    it._30天销售金额 = v._30天销售金额;
                    it._15天销售金额 = v._15天销售金额;
                    it._5天销售金额 = v._5天销售金额;
                    it._本期平均销售金额 = v._本期平均销售金额;

                    double d呆滞金额 = 0;
                    // 计算呆滞
                    switch (it._仓库类型)
                    {
                        case enum仓库类型.中转仓:
                            var day20Items = it.Items.Where(x => x._天数 == 20);
                            it._20天入库金额 = day20Items.Select(x => x._入库金额).Sum();
                            it._20天出库金额 = day20Items.Select(x => x._出库金额).Sum();
                            d呆滞金额 = it._期末金额 - it._20天入库金额 - it._20天出库金额;

                            it._本期呆滞 = d呆滞金额 < 0 ? 0 : d呆滞金额;

                            if (it._20天入库金额 == 0)
                            {
                                it._本期死库 = it._本期呆滞;
                            }
                            break;
                        case enum仓库类型.虚拟仓海外仓:
                            d呆滞金额 = it._期末金额 - it._本期平均销售金额 * 3 - it._入库金额;
                            it._本期呆滞 = d呆滞金额 < 0 ? 0 : d呆滞金额;

                            if (it._出库金额 == 0)
                            {
                                it._本期死库 = it._本期呆滞;
                            }
                            break;
                        case enum仓库类型.直发仓:
                            d呆滞金额 = it._期末金额 - it._本期平均销售金额 * 2 - (it._入库金额 < 0 ? 0 : it._入库金额);
                            it._本期呆滞 = d呆滞金额 < 0 ? 0 : d呆滞金额;

                            break;
                        case enum仓库类型.耗材仓:
                            var day60Items = it.Items.Where(x => x._天数 == 60);
                            it._60天入库金额 = day60Items.Select(x => x._入库金额).Sum();
                            it._60天出库金额 = day60Items.Select(x => x._出库金额).Sum();
                            d呆滞金额 = it._期末金额 - it._60天入库金额 - it._60天出库金额;
                            it._本期呆滞 = d呆滞金额 < 0 ? 0 : d呆滞金额;
                            if (it._出库金额 == 0)
                            {
                                it._本期死库 = it._本期呆滞;
                            }

                            break;
                        default:
                            break;
                    }
                }
                var a = 1;
            });
            //var list有效总账项 = list总账明细.Where(x => x._退换或弃置 == false).ToList();
            //var skus = list有效总账项.Select(x => x._仓库店铺SKU).Distinct().ToList();


            //for (var i = 0; i < skus.Count - 1; i++)
            //{
            //    var sku = skus[i];

            //    var refItems = new List<_库存出入总账>();
            //    for (var idx = list有效总账项.Count - 1; idx >= 0; idx--)
            //    {
            //        var it = list有效总账项[idx];
            //        if (it._仓库店铺SKU == sku)
            //        {
            //            refItems.Add(it);
            //            list有效总账项.RemoveAt(idx);
            //        }
            //    }

            //    var defItem = refItems[0];
            //    var dit = new _总账数据透视() { SKU = defItem.SKU, _仓库 = defItem._仓库, _店铺 = defItem._店铺, _仓库店铺SKU = defItem._仓库店铺SKU };
            //    dit.Items = refItems;

            //    dit._期初数量 = refItems.Select(x => x._期初数量).Sum();
            //    dit._期末数量 = refItems.Select(x => x._期末数量).Sum();
            //    dit._期初金额 = refItems.Select(x => x._期初金额).Sum();
            //    dit._期末金额 = refItems.Select(x => x._期末金额).Sum();
            //    dit._出库数量 = refItems.Select(x => x._出库数量).Sum();
            //    dit._出库金额 = refItems.Select(x => x._出库金额).Sum();
            //    dit._入库数量 = refItems.Select(x => x._入库数量).Sum();
            //    dit._入库金额 = refItems.Select(x => x._入库金额).Sum();

            //    list数据透视.Add(dit);
            //}
            #endregion

            Directory.Delete(currentTmpFolder, true);
            var exportFolder = fileSetting.GenerateTemporaryFolder("export");
            var exportFilePath = Path.Combine(exportFolder, Guid.NewGuid().ToString("N") + ".xlsx");

            #region 导出表格
            using (ExcelPackage package = new ExcelPackage(new FileInfo(exportFilePath)))
            {
                var workbox = package.Workbook;

                {
                    var sheet = workbox.Worksheets.Add("呆滞明细");
                    sheet.Cells[1, 1].Value = "仓库";
                    sheet.Cells[1, 2].Value = "店铺";
                    sheet.Cells[1, 3].Value = "SKU";
                    sheet.Cells[1, 4].Value = "仓库店铺SKU";
                    sheet.Cells[1, 5].Value = "期初数量";
                    sheet.Cells[1, 6].Value = "期末数量";
                    sheet.Cells[1, 7].Value = "期初金额";
                    sheet.Cells[1, 8].Value = "期末金额";
                    sheet.Cells[1, 9].Value = "出库数量";
                    sheet.Cells[1, 10].Value = "出库金额";
                    sheet.Cells[1, 11].Value = "入库数量";
                    sheet.Cells[1, 12].Value = "入库金额";
                    sheet.Cells[1, 13].Value = "仓库类型";
                    sheet.Cells[1, 14].Value = "销售金额";
                    sheet.Cells[1, 15].Value = "本期平均销售金额";
                    //
                    sheet.Cells[1, 16].Value = "20天出库金额";
                    sheet.Cells[1, 17].Value = "20天入库金额";
                    sheet.Cells[1, 18].Value = "30天出库金额";
                    sheet.Cells[1, 19].Value = "30天入库金额";
                    sheet.Cells[1, 20].Value = "60天出库金额";
                    sheet.Cells[1, 21].Value = "60天入库金额";
                    sheet.Cells[1, 22].Value = "本期呆滞";
                    sheet.Cells[1, 23].Value = "本期死库";

                    for (int idx = 0, rowIndex = 2; idx < list数据透视.Count; idx++)
                    {
                        var data = list数据透视[idx];
                        sheet.Cells[rowIndex, 1].Value = data._仓库;
                        sheet.Cells[rowIndex, 2].Value = data._店铺;
                        sheet.Cells[rowIndex, 3].Value = data.SKU;
                        sheet.Cells[rowIndex, 4].Value = data._仓库店铺SKU;
                        sheet.Cells[rowIndex, 5].Value = data._期初数量;
                        sheet.Cells[rowIndex, 6].Value = data._期末数量;
                        sheet.Cells[rowIndex, 7].Value = data._期初金额;
                        sheet.Cells[rowIndex, 8].Value = data._期末金额;
                        sheet.Cells[rowIndex, 9].Value = data._出库数量;
                        sheet.Cells[rowIndex, 10].Value = data._出库金额;
                        sheet.Cells[rowIndex, 11].Value = data._入库数量;
                        sheet.Cells[rowIndex, 12].Value = data._入库金额;
                        sheet.Cells[rowIndex, 13].Value = data._仓库类型;
                        sheet.Cells[rowIndex, 14].Value = data._销售金额;
                        sheet.Cells[rowIndex, 15].Value = data._本期平均销售金额;
                        // 
                        sheet.Cells[rowIndex, 16].Value = data._20天出库金额;
                        sheet.Cells[rowIndex, 17].Value = data._20天入库金额;
                        sheet.Cells[rowIndex, 18].Value = data._30天出库金额;
                        sheet.Cells[rowIndex, 19].Value = data._30天入库金额;
                        sheet.Cells[rowIndex, 20].Value = data._60天出库金额;
                        sheet.Cells[rowIndex, 21].Value = data._60天入库金额;
                        sheet.Cells[rowIndex, 22].Value = data._本期呆滞;
                        sheet.Cells[rowIndex, 23].Value = data._本期死库;

                        rowIndex++;
                    }
                }

                //{
                //    var sheet1 = workbox.Worksheets.Add("数据分析");

                //}

                package.Save();
            }
            #endregion

            //return new List<_库存出入总账>();
            return Unit.Value;
        }
    }

    public class _总账数据透视1
    {
        public string _仓库 { get; set; }
        public string _店铺 { get; set; }
        public string SKU { get; set; }
        public string _仓库店铺SKU { get; set; }
        public enum仓库类型? _仓库类型 { get; set; }
        public List<_库存出入总账> Items { get; set; } = new List<_库存出入总账>();

        public int _期初数量 { get; set; }
        public int _期末数量 { get; set; }
        public double _期初金额 { get; set; }
        public double _期末金额 { get; set; }
        public int _出库数量 { get; set; }
        public double _出库金额 { get; set; }
        public int _入库数量 { get; set; }
        public double _入库金额 { get; set; }

        public double _销售金额 { get; set; }
        public double _30天销售金额 { get; set; }
        public double _15天销售金额 { get; set; }
        public double _5天销售金额 { get; set; }
        public double _本期平均销售金额 { get; set; }


        public double _20天出库金额 { get; set; }
        public double _20天入库金额 { get; set; }
        public double _30天出库金额 { get; set; }
        public double _30天入库金额 { get; set; }
        public double _60天出库金额 { get; set; }
        public double _60天入库金额 { get; set; }

        public double _本期呆滞 { get; set; }
        public double? _本期死库 { get; set; }


        public void _计算几个数量金额信息()
        {
            _期初数量 = Items.Select(x => x._期初数量).Sum();
            _期末数量 = Items.Select(x => x._期末数量).Sum();
            _期初金额 = Items.Select(x => x._期初金额).Sum();
            _期末金额 = Items.Select(x => x._期末金额).Sum();
            _出库数量 = Items.Select(x => x._出库数量).Sum();
            _出库金额 = Items.Select(x => x._出库金额).Sum();
            _入库数量 = Items.Select(x => x._入库数量).Sum();
            _入库金额 = Items.Select(x => x._入库金额).Sum();
        }
    }

}
