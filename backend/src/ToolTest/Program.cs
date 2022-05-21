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

            //var d =DateTime.Parse("2022-03-01T02:46:46+00:00").ToString("yyyy-MM-dd");
            //var aaa = 1;

            #region 赔偿退货订单报表
            //var _退货赔偿订单文件夹 = "C://Users/Leon/Desktop/赔偿退货订单";
            var __退货赔偿订单基础数据文件夹 = "D://Favorite/退货赔偿订单基础数据";
            //var p赔偿订单 = $"{_退货赔偿订单文件夹}/亚马逊赔偿3.17-3.25.xlsx";
            //var p退货订单 = $"{_退货赔偿订单文件夹}/亚马逊退货订单3.17-3.25.xlsx";
            //var p退货订单 = @"C:\Users\Leon\Desktop\退货订单3.17-4.30原数据.xlsx";
            //var p退货订单 = @"C:\Users\Leon\Desktop\退货订单3.17-4.30原数据(4).xlsx";
            //var p赔偿订单 = @"C:\Users\Leon\Desktop\3.17-4.30赔偿订单原数据.xlsx";
            //var p赔偿订单 = @"C:\Users\Leon\Desktop\亚马逊赔偿21年5.19-22年5.20.xlsx";
            var p退货订单 = @"C:\Users\Leon\Desktop\退货订单3.17-4.30.xlsx";
            //var p退货订单 = @"C:\Users\Leon\Desktop\测试.xlsx";
            var p汇率匹配 = @"C:\Users\Leon\Desktop\4月汇率(1).xlsx";
            var p处理方案 = $"{__退货赔偿订单基础数据文件夹}/处理方案匹配表.xlsx";
            var p部门映射 = $"{__退货赔偿订单基础数据文件夹}/店铺-运营配置表.xlsx";
            var p店铺更名匹配 = $"{__退货赔偿订单基础数据文件夹}/站点更名匹配表.xlsx";
            var pMSKU2SKU = $"{__退货赔偿订单基础数据文件夹}/MSKU-SKU.xlsx";
            var pSKU价格匹配 = $"{__退货赔偿订单基础数据文件夹}/SKU对应采购成本价.xlsx";
            var handler = new CompensationHandler(null, p退货订单, p处理方案, p部门映射, p店铺更名匹配, pMSKU2SKU, pSKU价格匹配, p汇率匹配, exportFolder);
            var ms = await handler.Handle();
            saveExport(ms, "export.xlsx");
            #endregion

            #region 亚马逊索赔
            ////var p赔偿订单 = @"C:\Users\Leon\Desktop\模拟\3月份亚马逊赔偿报表.xlsx";
            //var __亚马逊索赔基础数据文件夹 = "D://Favorite/退货赔偿订单基础数据";
            //var p店铺更名匹配 = $"{__亚马逊索赔基础数据文件夹}/站点更名匹配表.xlsx";
            //var p赔偿订单 = @"C:\Users\Leon\Desktop\领星4月份亚马逊赔偿.xlsx";
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
    }
}
