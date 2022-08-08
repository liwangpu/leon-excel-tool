using ExcelTool.Domain.Handler;
using ExcelTool.Domain.Handler.Compensations;
using Flurl.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToolTest
{
    internal class Program
    {
        //public static string exportFolder = @"C:\Users\Leon\Desktop\ExcelToolExport";
        public static string exportFolder = @"C:\Users\User\Desktop\ExcelToolExport";
        public static string _基础数据文件夹 = @"C:\Users\User\Desktop\ExcelToolBasicFile";
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
            //object str = null;
            //var aaa = $"{str}";
            //var a1 = 1;

            var ppp = @"C:\Users\User\Desktop\亚马逊Vogvigo日本站.csv";
            string text = System.IO.File.ReadAllText(ppp, Encoding.UTF8);
            StreamWriter sw_CSV = new StreamWriter(ppp, false, Encoding.UTF8);
            //sw_CSV.
            var aaa = 1;

            ExcelTextFormat format = new ExcelTextFormat();
            format.Delimiter = ';';
            format.Culture = new CultureInfo(Thread.CurrentThread.CurrentCulture.ToString());
            //format.Culture.DateTimeFormat.ShortDatePattern = "dd-mm-yyyy";
            //format.Encoding = new UTF8Encoding();

            //read the CSV file from disk
            //FileInfo file = new FileInfo("C:\\CSVDemo.csv");

            //or if you use asp.net, get the relative path
            FileInfo file = new FileInfo(ppp);

            //create a new Excel package
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(@"C:\Users\User\Desktop\ExcelToolExport\test.xlsx")))
            {
                //create a WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                //load the CSV data into cell A1
                worksheet.Cells["A1"].LoadFromText(file, format);
                excelPackage.Save();
            }

            var aaa11 = 1;
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
            //var p空海运差异表 = @"C:\Users\User\Desktop\7月份空海运差异报表.xlsx";
            //var p空海运差异表 = @"C:\Users\Leon\Desktop\11.xlsx";
            //var handler = new FreightChargeHandler(p空海运差异表, exportFolder);
            //var ms = await handler.Handle();
            //saveExport(ms, "export1.xlsx");
            #endregion

            #region 亚马逊店铺利润表
            ////var p店铺流水压缩包 = @"C:\Users\Leon\Desktop\亚马逊流水汇总表(第一张表).zip";
            //var p店铺流水压缩包 = @"C:\Users\User\Desktop\aaa.zip";
            //var p亚马逊汇总表各站点标题匹配表 = @"C:\Users\User\Desktop\亚马逊店铺流水\亚马逊汇总表各站点标题.xlsx";
            //var p亚马逊店铺汇总表 = @"C:\Users\User\Desktop\亚马逊店铺流水\店铺汇总表.xlsx";
            ////var p店铺ASIN负责表 = @"C:\Users\Leon\Desktop\源数据表\店铺-ASIN负责人表.xlsx";
            ////var p平台费用表 = @"C:\Users\Leon\Desktop\源数据表\6月平台费用(2) - 副本.xlsx";
            //var handler = new AmazonShopProfit(p店铺流水压缩包, p亚马逊汇总表各站点标题匹配表, p亚马逊店铺汇总表, null, null, exportFolder);
            //var ms = await handler.Handle();
            //saveExport(ms, "export.xlsx");
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
