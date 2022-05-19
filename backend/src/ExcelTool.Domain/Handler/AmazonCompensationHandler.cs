using ExcelTool.Domain.Models.AmazonCompensations;
using ExcelTool.Domain.Models.Commons;
using ExcelTool.Domain.Utils;
using Npoi.Mapper;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelTool.Domain.Handler.Compensations
{
    public class AmazonCompensationHandler
    {
        protected string path赔偿订单文件;
        protected string path店铺更名匹配文件;
        protected string folder临时文件夹;

        #region ctor
        protected AmazonCompensationHandler()
        {

        }
        public AmazonCompensationHandler(string p赔偿订单文件, string p店铺更名匹配文件, string f临时文件夹)
                  : this()
        {
            path赔偿订单文件 = p赔偿订单文件;
            path店铺更名匹配文件 = p店铺更名匹配文件;
            folder临时文件夹 = f临时文件夹;
        }
        #endregion

        public async Task<MemoryStream> Handle()
        {
            var list赔偿订单 = new List<_赔偿报表源数据>();
            var list站点更名匹配 = new List<_站点更名匹配表>();
            var list_统计结果 = new List<_统计结果>();
            var dict唯一值to国家 = new Dictionary<string, string>();
            var dict唯一值to统计项 = new Dictionary<string, _统计结果>();
            var util店铺更名工具 = new StoreNameHelper();
            //var exportFolder = FolderHelper.GenerateTemporaryFolder(folder临时文件夹);

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

            if (!string.IsNullOrEmpty(path赔偿订单文件))
            {
                var mapper = new Mapper(path赔偿订单文件);
                var sheetDatas = mapper.Take<_赔偿报表源数据>().Select(x => x.Value).ToList();
                sheetDatas.ForEach(it =>
                {
                    it._店铺 = util店铺更名工具._标准化店铺名称(it._国家, it._领星店铺名称);
                    it._数据处理();
                });
                list赔偿订单.AddRange(sheetDatas);
            }
            #endregion

            #region 数据处理
            list赔偿订单.ForEach(it =>
            {
                //if (it._赔偿编号 == "3936776422")
                //{
                //    var aaaaa = 1;
                //}
                if (dict唯一值to国家.ContainsKey(it.Key))
                {
                    // 判断国家是不是一个,不是的话 只取一个国家作为计算
                    if (it._国家 == dict唯一值to国家[it.Key])
                    {
                        var ditem = dict唯一值to统计项[it.Key];
                        ditem._合并统计(it);
                    }
                }
                else
                {
                    var ditem = new _统计结果(it);
                    dict唯一值to统计项[it.Key] = ditem;
                    dict唯一值to国家[it.Key] = it._国家;
                    list_统计结果.Add(ditem);
                }
            });
            #endregion

            var exportFilePath = Path.Combine(folder临时文件夹, "result.xlsx");

            #region 表格打印
            using (ExcelPackage package = new ExcelPackage(new FileInfo(exportFilePath)))
            {
                var workbox = package.Workbook;
                // 打印汇总表
                {
                    var sheet = workbox.Worksheets.Add($"汇总表");

                    #region 标题行
                    sheet.Cells[1, 1].Value = "统计编号";
                    sheet.Cells[1, 2].Value = "统计个数";
                    sheet.Cells[1, 3].Value = "店铺";
                    sheet.Cells[1, 4].Value = "国家";
                    sheet.Cells[1, 5].Value = "时间";
                    sheet.Cells[1, 6].Value = "赔偿编号";
                    sheet.Cells[1, 7].Value = "订单号";
                    sheet.Cells[1, 8].Value = "原因";
                    sheet.Cells[1, 9].Value = "MSKU";
                    sheet.Cells[1, 10].Value = "FNSKU";
                    sheet.Cells[1, 11].Value = "ASIN";
                    sheet.Cells[1, 12].Value = "标题";
                    sheet.Cells[1, 13].Value = "状况";
                    sheet.Cells[1, 14].Value = "币种";
                    sheet.Cells[1, 15].Value = "每件商品赔偿金额";
                    sheet.Cells[1, 16].Value = "总金额";
                    sheet.Cells[1, 17].Value = "赔偿数量（现金）";
                    sheet.Cells[1, 18].Value = "赔偿数量（库存）";
                    sheet.Cells[1, 19].Value = "赔偿数量（总计）";
                    sheet.Cells[1, 20].Value = "原始赔偿编号";
                    sheet.Cells[1, 21].Value = "原始赔偿类型";

                    #endregion

                    #region 数据行
                    var startRow = 2;
                    var rowIndex = startRow;
                    for (int idx = 0; idx < list_统计结果.Count; idx++)
                    {
                        var data = list_统计结果[idx];
                        sheet.Cells[rowIndex, 1].Value = data.UniqCode;
                        sheet.Cells[rowIndex, 2].Value = data.Items.Count;
                        sheet.Cells[rowIndex, 3].Value = data._店铺;
                        sheet.Cells[rowIndex, 4].Value = data._国家;
                        sheet.Cells[rowIndex, 5].Value = data._时间;
                        sheet.Cells[rowIndex, 6].Value = data._赔偿编号;
                        sheet.Cells[rowIndex, 7].Value = data._订单号;
                        sheet.Cells[rowIndex, 8].Value = data._原因;
                        sheet.Cells[rowIndex, 9].Value = data.MSKU;
                        sheet.Cells[rowIndex, 10].Value = data.FNSKU;
                        sheet.Cells[rowIndex, 11].Value = data.ASIN;
                        sheet.Cells[rowIndex, 12].Value = data._标题;
                        sheet.Cells[rowIndex, 13].Value = data._状况;
                        sheet.Cells[rowIndex, 14].Value = data._币种;
                        sheet.Cells[rowIndex, 15].Value = data._每件商品赔偿金额;
                        sheet.Cells[rowIndex, 16].Value = data._总金额;
                        sheet.Cells[rowIndex, 17].Value = data._赔偿数量_现金;
                        sheet.Cells[rowIndex, 18].Value = data._赔偿数量_库存;
                        sheet.Cells[rowIndex, 19].Value = data._赔偿数量_总计;
                        sheet.Cells[rowIndex, 20].Value = data._原始赔偿编号;
                        sheet.Cells[rowIndex, 21].Value = data._原始赔偿类型;

                        rowIndex++;
                    }
                    #endregion
                }

                // 打印详情表
                {
                    var sheet = workbox.Worksheets.Add($"详情表");
                    #region 标题行
                    sheet.Cells[1, 1].Value = "统计编号";
                    sheet.Cells[1, 2].Value = "领星店铺名称";
                    sheet.Cells[1, 3].Value = "南棠店铺名称";
                    sheet.Cells[1, 4].Value = "国家";
                    sheet.Cells[1, 5].Value = "时间";
                    sheet.Cells[1, 6].Value = "赔偿编号";
                    sheet.Cells[1, 7].Value = "订单号";
                    sheet.Cells[1, 8].Value = "原因";
                    sheet.Cells[1, 9].Value = "MSKU";
                    sheet.Cells[1, 10].Value = "FNSKU";
                    sheet.Cells[1, 11].Value = "ASIN";
                    sheet.Cells[1, 12].Value = "标题";
                    sheet.Cells[1, 13].Value = "状况";
                    sheet.Cells[1, 14].Value = "币种";
                    sheet.Cells[1, 15].Value = "每件商品赔偿金额";
                    sheet.Cells[1, 16].Value = "总金额";
                    sheet.Cells[1, 17].Value = "赔偿数量（现金）";
                    sheet.Cells[1, 18].Value = "赔偿数量（库存）";
                    sheet.Cells[1, 19].Value = "赔偿数量（总计）";
                    sheet.Cells[1, 10].Value = "原始赔偿编号";
                    sheet.Cells[1, 21].Value = "原始赔偿类型";

                    #endregion

                    #region 数据行
                    var startRow = 2;
                    var rowIndex = startRow;
                    for (int idx = 0; idx < list_统计结果.Count; idx++)
                    {
                        var data = list_统计结果[idx];

                        data.Items.ForEach(it =>
                        {
                            sheet.Cells[rowIndex, 1].Value = data.UniqCode;
                            sheet.Cells[rowIndex, 2].Value = it._领星店铺名称;
                            sheet.Cells[rowIndex, 3].Value = it._店铺;
                            sheet.Cells[rowIndex, 4].Value = it._国家;
                            sheet.Cells[rowIndex, 5].Value = it._时间;
                            sheet.Cells[rowIndex, 6].Value = it._赔偿编号;
                            sheet.Cells[rowIndex, 7].Value = it._订单号;
                            sheet.Cells[rowIndex, 8].Value = it._原因;
                            sheet.Cells[rowIndex, 9].Value = it.MSKU;
                            sheet.Cells[rowIndex, 10].Value = it.FNSKU;
                            sheet.Cells[rowIndex, 11].Value = it.ASIN;
                            sheet.Cells[rowIndex, 12].Value = it._标题;
                            sheet.Cells[rowIndex, 13].Value = it._状况;
                            sheet.Cells[rowIndex, 14].Value = it._币种;
                            sheet.Cells[rowIndex, 15].Value = it._每件商品赔偿金额;
                            sheet.Cells[rowIndex, 16].Value = it._总金额;
                            sheet.Cells[rowIndex, 17].Value = it._赔偿数量_现金;
                            sheet.Cells[rowIndex, 18].Value = it._赔偿数量_库存;
                            sheet.Cells[rowIndex, 19].Value = it._赔偿数量_总计;
                            sheet.Cells[rowIndex, 20].Value = it._原始赔偿编号;
                            sheet.Cells[rowIndex, 21].Value = it._原始赔偿类型;
                            rowIndex++;
                        });
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

    class _统计结果
    {
        public string _店铺 { get; set; }
        public string _国家 { get; set; }
        public string _时间 { get; set; }
        public string _赔偿编号 { get; set; }
        public string _订单号 { get; set; }
        public string _原因 { get; set; }
        public string MSKU { get; set; }
        public string ASIN { get; set; }
        public string FNSKU { get; set; }
        public string _标题 { get; set; }
        public string _状况 { get; set; }
        public string _币种 { get; set; }
        public decimal _每件商品赔偿金额 { get; set; }
        public decimal _总金额 { get; set; }
        public decimal _赔偿数量_现金 { get; set; }
        public decimal _赔偿数量_库存 { get; set; }
        public decimal _赔偿数量_总计 { get; set; }
        public string _原始赔偿编号 { get; set; }
        public string _原始赔偿类型 { get; set; }
        public string UniqCode { get; protected set; }
        public List<_赔偿报表源数据> Items { get; protected set; } = new List<_赔偿报表源数据>();

        protected _统计结果()
        {
            UniqCode = Guid.NewGuid().ToString("N").ToUpper();
        }

        public _统计结果(_赔偿报表源数据 data)
            : this()
        {
            _店铺 = data._店铺;
            _国家 = data._国家;
            _时间 = data._时间;
            _赔偿编号 = data._赔偿编号;
            _订单号 = data._订单号;
            _原因 = data._原因;
            MSKU = data.MSKU;
            ASIN = data.ASIN;
            FNSKU = data.FNSKU;
            _标题 = data._标题;
            _状况 = data._状况;
            _币种 = data._币种;
            _每件商品赔偿金额 = data._每件商品赔偿金额;
            _总金额 = data._总金额;
            _赔偿数量_现金 = data._赔偿数量_现金;
            _赔偿数量_库存 = data._赔偿数量_库存;
            _赔偿数量_总计 = data._赔偿数量_总计;
            _原始赔偿编号 = data._原始赔偿编号;
            _原始赔偿类型 = data._原始赔偿类型;
            Items.Add(data);
        }

        public void _合并统计(_赔偿报表源数据 data)
        {
            _总金额 += data._总金额;
            _赔偿数量_现金 += data._赔偿数量_现金;
            _赔偿数量_库存 += data._赔偿数量_库存;
            _赔偿数量_总计 += data._赔偿数量_总计;
            Items.Add(data);
        }
    }

}
