using ExcelTool.Domain.Handler;
using ExcelTool.Domain.Handler.Compensations;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ToolTest
{
    internal class Program
    {
        public static string exportFolder = @"C:\Users\Leon\Desktop\ExcelToolExport";
        public static string _基础数据文件夹 = @"C:\Users\Leon\Desktop\ExcelToolBasicFile";
        public static void Main(string[] args)
        {
            if (!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }
            if (!Directory.Exists(_基础数据文件夹))
            {
                Directory.CreateDirectory(_基础数据文件夹);
            }
            MainAsync(args).GetAwaiter().GetResult();
            Console.WriteLine("执行完毕!");
            Console.ReadKey();
        }

        public static async Task MainAsync(string[] args)
        {
            //await downloadBasicFiles();

            #region 赔偿退货订单报表
            //var p退货订单 = @"C:\Users\User\Desktop\退货5月整月(1).xlsx";
            //var p赔偿订单 = @"C:\Users\User\Desktop\赔偿5月整月(1).xlsx";
            //var p汇率匹配 = $"{_基础数据文件夹}/汇率匹配表.xlsx";
            //var p处理方案 = $"{_基础数据文件夹}/退货赔偿处理方案.xlsx";
            //var p部门映射 = $"{_基础数据文件夹}/店铺运营配置表.xlsx";
            //var p店铺更名匹配 = $"{_基础数据文件夹}/领星店铺更名为南棠店铺匹配表.xlsx";
            //var pMSKU2SKU = $"{_基础数据文件夹}/MSKU_SKU匹配表.xlsx";
            //var pSKU价格匹配 = $"{_基础数据文件夹}/SKU_价格匹配表.xlsx";
            //var handler = new CompensationHandler(p赔偿订单, p退货订单, p处理方案, p部门映射, p店铺更名匹配, pMSKU2SKU, pSKU价格匹配, p汇率匹配, exportFolder);
            //var ms = await handler.Handle();
            //saveExport(ms, "export.xlsx");
            #endregion

            #region 亚马逊索赔
            //var p店铺更名匹配 = $"{_基础数据文件夹}/领星店铺更名为南棠店铺匹配表.xlsx";
            //var p赔偿订单 = @"C:\Users\Leon\Desktop\3月份索赔(2)(1).xlsx";
            //var handler = new AmazonCompensationHandler(p赔偿订单, p店铺更名匹配, exportFolder);
            //var ms = await handler.Handle();
            //saveExport(ms, "export.xlsx");
            #endregion

            #region 空海运差异报表
            //var p空海运差异表 = @"C:\Users\Leon\Desktop\6月份空海运差异.xlsx";
            ////var p空海运差异表 = @"C:\Users\Leon\Desktop\11.xlsx";
            //var handler = new FreightChargeHandler(p空海运差异表, exportFolder);
            //var ms = await handler.Handle();
            //saveExport(ms, "export1.xlsx");
            #endregion

            #region 亚马逊店铺利润表
            var p店铺ASIN负责表 = @"C:\Users\Leon\Desktop\源数据表\店铺-ASIN负责人表.xlsx";
            var p平台费用表 = @"C:\Users\Leon\Desktop\源数据表\6月平台费用(2) - 副本.xlsx";
            var handler = new AmazonShopProfit(p店铺ASIN负责表, p平台费用表, exportFolder);
            var ms = await handler.Handle();
            saveExport(ms, "export.xlsx");
            #endregion
        }

        public static void saveExport(MemoryStream ms, string fileName)
        {
            var filePath = Path.Combine(exportFolder, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (ms)
            {
                using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(file);
                }
            }

        }

        public static async Task downloadBasicFiles()
        {
            var uri = "http://1.116.37.43:3101/api/file";
            var fileNames = new List<string>()
            {
                 "领星店铺更名为南棠店铺匹配表.xlsx",
                 "店铺运营配置表.xlsx",
                 "退货赔偿处理方案.xlsx",
                 "MSKU_SKU匹配表.xlsx",
                 "SKU_价格匹配表.xlsx",
                 "汇率匹配表.xlsx"
            };

            foreach (var fk in fileNames)
            {
                var res = await $"{uri}/Check/{fk}".GetStringAsync();
                if (res == "true")
                {
                    using (var myWebClient = new WebClient())
                    {
                        await myWebClient.DownloadFileTaskAsync(new Uri($"{uri}/Download/{fk}"), Path.Combine(_基础数据文件夹, fk));
                    }
                }
            }
        }
    }
}
