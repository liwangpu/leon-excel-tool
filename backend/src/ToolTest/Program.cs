using ExcelTool.Domain.Handler.Compensations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ToolTest
{
    internal class Program
    {
        public static string exportFolder = @"C:\Users\Leon\Desktop\ExcelToolExport";
        public static void Main(string[] args)
        {
            if (!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }
            MainAsync(args).GetAwaiter().GetResult();
            Console.WriteLine("执行完毕!");
            Console.ReadKey();
        }

        public static async Task MainAsync(string[] args)
        {
            //var p赔偿订单 = @"D:\Favorite\3.26\亚马逊赔偿3.17-3.25.xlsx";
            //var p退货订单 = @"D:\Favorite\3.26\亚马逊退货订单3.17-3.25.xlsx";
            //var p处理方案 = @"D:\Favorite\3.26\处理方案匹配表.xlsx";
            //var p部门映射 = @"D:\Favorite\3.26\店铺-运营配置表.xlsx";
            //var handler = new CompensationHandler(p赔偿订单, p退货订单, p处理方案, p部门映射);
            //await handler.Handle();


            #region MyRegion
            //var p赔偿订单 = @"C:\Users\Leon\Desktop\模拟\3月份亚马逊赔偿报表.xlsx";
            var p店铺更名匹配 = @"C:\Users\Leon\Desktop\模拟\站点更名匹配表.xlsx";
            var p赔偿订单 = @"C:\Users\Leon\Desktop\真实\3月份亚马逊赔偿报表.xlsx";
            var handler = new AmazonCompensationHandler(p赔偿订单, p店铺更名匹配, exportFolder);
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
    }
}
