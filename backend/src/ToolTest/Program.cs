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
            var p退货订单 = @"C:\Users\Leon\Desktop\7-6\5月份退货订单21号导出排除12号导出.xlsx";
            var p赔偿订单 = @"C:\Users\Leon\Desktop\7-6\5月份亚马逊赔偿21号导出排除12号导出.xlsx";
            var p汇率匹配 = $"{_基础数据文件夹}/汇率匹配表.xlsx";
            var p处理方案 = $"{_基础数据文件夹}/退货赔偿处理方案.xlsx";
            var p部门映射 = $"{_基础数据文件夹}/店铺运营配置表.xlsx";
            var p店铺更名匹配 = $"{_基础数据文件夹}/领星店铺更名为南棠店铺匹配表.xlsx";
            var pMSKU2SKU = $"{_基础数据文件夹}/MSKU_SKU匹配表.xlsx";
            var pSKU价格匹配 = $"{_基础数据文件夹}/SKU_价格匹配表.xlsx";
            var handler = new CompensationHandler(p赔偿订单, p退货订单, p处理方案, p部门映射, p店铺更名匹配, pMSKU2SKU, pSKU价格匹配, p汇率匹配, exportFolder);
            var ms = await handler.Handle();
            saveExport(ms, "export.xlsx");
            #endregion

            #region 亚马逊索赔
            //var p店铺更名匹配 = $"{_基础数据文件夹}/领星店铺更名为南棠店铺匹配表.xlsx";
            //var p赔偿订单 = @"C:\Users\Leon\Desktop\3月份索赔(2)(1).xlsx";
            //var handler = new AmazonCompensationHandler(p赔偿订单, p店铺更名匹配, exportFolder);
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
