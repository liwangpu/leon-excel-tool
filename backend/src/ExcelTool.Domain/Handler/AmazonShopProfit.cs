using ExcelTool.Domain.Models.AmazonCompensations;
using ExcelTool.Domain.Models.AmazonShopProfits;
using ExcelTool.Domain.Models.Commons;
using ExcelTool.Domain.Utils;
using Npoi.Mapper;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelTool.Domain.Handler
{
    public class AmazonShopProfit
    {
        public string path店铺ASIN负责表;
        public string path平台费用表;
        protected string folder临时文件夹;

        #region ctor
        protected AmazonShopProfit()
        {

        }
        public AmazonShopProfit(string p店铺ASIN负责表, string p平台费用表, string f临时文件夹)
                  : this()
        {
            path店铺ASIN负责表 = p店铺ASIN负责表;
            path平台费用表 = p平台费用表;
            folder临时文件夹 = f临时文件夹;
        }
        #endregion

        public async Task<MemoryStream> Handle()
        {
            var list店铺ASIN负责人 = new List<_店铺ASIN负责人>();
            var list平台费用 = new List<_平台费用>();
            var map店铺负责人匹配数据 = new Dictionary<string, _店铺ASIN负责人匹配数据>();
            var map平台费用匹配数据 = new Dictionary<string, _平台费用>();

            #region 数据读取
            if (!string.IsNullOrEmpty(path平台费用表))
            {
                var mapper = new Mapper(path平台费用表);
                var sheetDatas = mapper.Take<_平台费用>().Select(x => x.Value).ToList();
                sheetDatas.ForEach(it =>
                {
                    it._店铺 = string.IsNullOrWhiteSpace(it._店铺) ? "" : it._店铺;
                    list平台费用.Add(it);
                    if (!string.IsNullOrWhiteSpace(it._店铺))
                    {
                        map平台费用匹配数据.Add(it._店铺, it);
                    }
                });
            }

            if (!string.IsNullOrEmpty(path店铺ASIN负责表))
            {
                var mapper = new Mapper(path店铺ASIN负责表);
                var sheetDatas = mapper.Take<_店铺ASIN负责人>().Select(x => x.Value).ToList();
                sheetDatas.ForEach(it =>
                {
                    it._店铺 = string.IsNullOrWhiteSpace(it._店铺) ? "" : it._店铺;
                    if (string.IsNullOrWhiteSpace(it._店铺))
                    {
                        return;
                    }
                    list店铺ASIN负责人.Add(it);
                    it._平台费用Data = map平台费用匹配数据.ContainsKey(it._店铺) ? map平台费用匹配数据[it._店铺] : null;
                    if (!map店铺负责人匹配数据.ContainsKey(it._店铺))
                    {
                        var mitem = new _店铺ASIN负责人匹配数据();
                        mitem.Datas.Add(it);
                        map店铺负责人匹配数据[it._店铺] = mitem;
                    }
                    else
                    {
                        var mitem = map店铺负责人匹配数据[it._店铺];
                        mitem.Datas.Add(it);
                    }
                });
            }
            #endregion

            #region 数据处理
            map店铺负责人匹配数据.ForEach(it =>
            {
                var mitem = it.Value;
                mitem.calcu计算销售额比例相关();
            });
            #endregion

            var exportFilePath = Path.Combine(folder临时文件夹, "result.xlsx");

            #region 表格打印
            using (ExcelPackage package = new ExcelPackage(new FileInfo(exportFilePath)))
            {
                var workbox = package.Workbook;
                // 打印汇总表
                {
                    var sheet = workbox.Worksheets.Add($"店铺利润表");

                    #region 标题行
                    sheet.Cells[1, 1].Value = "店铺";
                    sheet.Cells[1, 2].Value = "组别";
                    sheet.Cells[1, 3].Value = "币种";
                    sheet.Cells[1, 4].Value = "ASIN负责人";
                    sheet.Cells[1, 5].Value = "销售总额RMB";
                    sheet.Cells[1, 6].Value = "订单汇率";
                    sheet.Cells[1, 7].Value = "扣除费用所得(原币)";
                    sheet.Cells[1, 8].Value = "扣除费用所得(RMB)";
                    sheet.Cells[1, 9].Value = "商品成本RMB";
                    sheet.Cells[1, 10].Value = "头程运费";
                    sheet.Cells[1, 11].Value = "包装成本RMB";
                    sheet.Cells[1, 12].Value = "快递费";
                    sheet.Cells[1, 13].Value = "订单利润RMB";
                    sheet.Cells[1, 14].Value = "美元汇率";
                    sheet.Cells[1, 15].Value = "销售总额$";
                    sheet.Cells[1, 16].Value = "扣除费用所得$";
                    sheet.Cells[1, 17].Value = "美金利润$";
                    sheet.Cells[1, 18].Value = "利润率";
                    sheet.Cells[1, 19].Value = "销售额比例";
                    sheet.Cells[1, 20].Value = "广告";
                    sheet.Cells[1, 21].Value = "秒杀";
                    sheet.Cells[1, 22].Value = "退款";
                    sheet.Cells[1, 23].Value = "长期仓储费";
                    sheet.Cells[1, 24].Value = "仓储费";
                    sheet.Cells[1, 25].Value = "库存放置费";
                    sheet.Cells[1, 26].Value = "移除费";
                    sheet.Cells[1, 27].Value = "弃置";
                    sheet.Cells[1, 28].Value = "月租";
                    sheet.Cells[1, 29].Value = "索赔";
                    sheet.Cells[1, 30].Value = "自报加项";
                    sheet.Cells[1, 31].Value = "刷单(美金)";
                    sheet.Cells[1, 32].Value = "费用合计";
                    sheet.Cells[1, 33].Value = "店铺毛利$";
                    #endregion

                    #region 数据行
                    var startRow = 2;
                    var rowIndex = startRow;
                    for (int idx = 0; idx < list店铺ASIN负责人.Count; idx++)
                    {
                        var data = list店铺ASIN负责人[idx];
                        sheet.Cells[rowIndex, 1].Value = data._店铺;
                        sheet.Cells[rowIndex, 2].Value = data._组别;
                        sheet.Cells[rowIndex, 3].Value = data._币种;
                        sheet.Cells[rowIndex, 4].Value = data.ASIN负责人;
                        sheet.Cells[rowIndex, 5].Value = data._销售总额_RMB;
                        sheet.Cells[rowIndex, 6].Value = data._订单汇率;
                        sheet.Cells[rowIndex, 7].Value = data._扣除费用所得_原币;
                        sheet.Cells[rowIndex, 8].Value = data._扣除费用所得_RMB;
                        sheet.Cells[rowIndex, 9].Value = data._商品成本RMB;
                        sheet.Cells[rowIndex, 10].Value = data._头程运费;
                        sheet.Cells[rowIndex, 11].Value = data._包装成本_RMB;

                        sheet.Cells[rowIndex, 12].Value = data._快递费;
                        sheet.Cells[rowIndex, 13].Value = data._订单利润_RMB;
                        sheet.Cells[rowIndex, 14].Value = data._美元汇率;
                        sheet.Cells[rowIndex, 15].Value = data._销售总额_美元;
                        sheet.Cells[rowIndex, 16].Value = data._扣除费用所得_美元;
                        sheet.Cells[rowIndex, 17].Value = data._美金利润_美元;
                        sheet.Cells[rowIndex, 18].Value = data._利润率;
                        sheet.Cells[rowIndex, 19].Value = data._销售额比例;
                        sheet.Cells[rowIndex, 20].Value = data._广告;
                        sheet.Cells[rowIndex, 21].Value = data._秒杀;
                        sheet.Cells[rowIndex, 22].Value = data._退款;
                        sheet.Cells[rowIndex, 23].Value = data._长期仓储费;
                        sheet.Cells[rowIndex, 24].Value = data._仓储费;
                        sheet.Cells[rowIndex, 25].Value = data._库存放置费;
                        sheet.Cells[rowIndex, 26].Value = data._移除费;
                        sheet.Cells[rowIndex, 27].Value = data._弃置;
                        sheet.Cells[rowIndex, 28].Value = data._月租;
                        sheet.Cells[rowIndex, 29].Value = data._索赔;
                        sheet.Cells[rowIndex, 30].Value = data._自报加项;
                        sheet.Cells[rowIndex, 31].Value = data._刷单_美金;
                        sheet.Cells[rowIndex, 32].Value = data._费用合计;
                        sheet.Cells[rowIndex, 33].Value = data._店铺毛利率;

                        rowIndex++;
                    }
                    #endregion

                    #region 样式
                    sheet.Column(1).Width = 30;
                    sheet.Column(2).Width = 30;
                    sheet.Column(3).Width = 9;
                    sheet.Column(4).Width = 12;
                    for (var i = 5; i <= 33; i++)
                    {
                        sheet.Column(i).Width = 14;
                    }
                    #endregion
                }

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

    class _店铺ASIN负责人匹配数据
    {
        public decimal _销售总额_RMB { get; protected set; }
        public List<_店铺ASIN负责人> Datas { get; set; } = new List<_店铺ASIN负责人>();
        public void calcu计算销售额比例相关()
        {
            _销售总额_RMB = Datas.Select(x => x._销售总额_RMB).Sum();
            Datas.ForEach(it =>
            {
                if (Datas.Count == 1)
                {
                    it._销售额比例 = 1;
                    return;
                }
                it._销售额比例 = _销售总额_RMB > 0 ? (it._销售总额_RMB * 10000) / (_销售总额_RMB * 10000) : 0;

            });
        }

    }
}