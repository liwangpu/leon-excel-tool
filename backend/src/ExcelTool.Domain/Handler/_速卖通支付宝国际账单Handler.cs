using CsvHelper;
using CsvHelper.Configuration;
using ExcelTool.Domain.Models.AliExpress;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ExcelTool.Domain.Handler
{
    public class _速卖通支付宝国际账单Handler
    {

        protected string folder临时文件夹;
        protected string folder压缩包文件夹;

        public _速卖通支付宝国际账单Handler(string p压缩包文件夹, string f临时文件夹)
        {
            folder压缩包文件夹 = p压缩包文件夹;
            folder临时文件夹 = f临时文件夹;
        }

        public async Task<MemoryStream> Handle()
        {
            var zipFilePaths = Directory.GetFiles(folder压缩包文件夹, "*.zip");
            var datas = new List<_速卖通支付宝国际账单汇总表>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                //HasHeaderRecord = false,
                MissingFieldFound = null
            };
            var csvTotalFolder = Path.Combine(folder临时文件夹, "csv");
            if (!Directory.Exists(csvTotalFolder))
            {
                Directory.CreateDirectory(csvTotalFolder);
            }
            foreach (var p in zipFilePaths)
            {
                var _店铺名称 = Path.GetFileNameWithoutExtension(p);
                var csvFolder = Path.Combine(csvTotalFolder, _店铺名称);
                if (!Directory.Exists(csvFolder))
                {
                    Directory.CreateDirectory(csvFolder);
                }
                else
                {
                    Directory.Delete(csvFolder, true);
                }
                ZipFile.ExtractToDirectory(p, csvFolder, true);
                var csvFiles = Directory.GetFiles(csvFolder, "*.csv", SearchOption.AllDirectories);
                foreach (var cp in csvFiles)
                {
                    using (var reader = new StreamReader(cp))
                    using (var csv = new CsvReader(reader, config))
                    {
                        var inDataRow = false;
                        while (csv.Read())
                        {
                            string stringField = csv.GetField<string>(0);
                            if (!inDataRow)
                            {
                                if (csv.GetField<string>(0) == "Time" && csv.GetField<string>(1) == "Type" && csv.GetField<string>(2) == "Reference No." && csv.GetField<string>(3) == "Alipay Transaction No.")
                                {
                                    inDataRow = true;
                                }
                            }
                            if (inDataRow)
                            {
                                var record = csv.GetRecord<_速卖通支付宝国际账单汇总表>();
                                if (record != null)
                                {
                                    record._店铺名称 = _店铺名称;
                                    datas.Add(record);
                                }

                            }
                        }

                    }
                }
            }
            if (Directory.Exists(csvTotalFolder))
            {
                Directory.Delete(csvTotalFolder, true);
            }


            var exportFilePath = Path.Combine(folder临时文件夹, "result.xlsx");

            #region 表格打印
            using (var package = new ExcelPackage(new FileInfo(exportFilePath)))
            {
                var workbox = package.Workbook;
                // 打印汇总表
                {
                    var sheet = workbox.Worksheets.Add($"汇总表");

                    #region 标题行
                    sheet.Cells[1, 1].Value = "店铺名称";
                    sheet.Cells[1, 2].Value = "Time";
                    sheet.Cells[1, 3].Value = "Type";
                    sheet.Cells[1, 4].Value = "Reference No.";
                    sheet.Cells[1, 5].Value = "Alipay Transaction No.";
                    sheet.Cells[1, 6].Value = "Amount";
                    sheet.Cells[1, 7].Value = "Balance";
                    sheet.Cells[1, 8].Value = "Currency";
                    sheet.Cells[1, 9].Value = "Remarks";
                    sheet.Cells[1, 10].Value = "Merchant Transaction No.";
                    sheet.Cells[1, 11].Value = "Product Name";
                    sheet.Cells[1, 12].Value = "Counterparty Name";
                    sheet.Cells[1, 13].Value = "Counterparty Account";
                    sheet.Cells[1, 14].Value = "Exchange Rate";

                    #endregion

                    #region 数据行
                    var startRow = 2;
                    var rowIndex = startRow;
                    for (int idx = 0; idx < datas.Count; idx++)
                    {
                        var data = datas[idx];
                        sheet.Cells[rowIndex, 1].Value = data._店铺名称;
                        sheet.Cells[rowIndex, 2].Value = data.Time;
                        sheet.Cells[rowIndex, 3].Value = data.Type;
                        sheet.Cells[rowIndex, 4].Value = data.ReferenceNo;
                        sheet.Cells[rowIndex, 5].Value = data.AlipayTransactionNo;
                        sheet.Cells[rowIndex, 6].Value = data.Amount;
                        sheet.Cells[rowIndex, 7].Value = data.Balance;
                        sheet.Cells[rowIndex, 8].Value = data.Currency;
                        sheet.Cells[rowIndex, 9].Value = data.Remarks;
                        sheet.Cells[rowIndex, 10].Value = data.MerchantTransactionNo;
                        sheet.Cells[rowIndex, 11].Value = data.ProductName;
                        sheet.Cells[rowIndex, 12].Value = data.CounterpartyName;
                        sheet.Cells[rowIndex, 13].Value = data.CounterpartyAccount;
                        sheet.Cells[rowIndex, 14].Value = data.ExchangeRate;

                        rowIndex++;
                    }
                    #endregion

                    #region 样式
                    sheet.Column(1).Width = 22;
                    sheet.Column(2).Width = 20;
                    sheet.Column(3).Width = 15;
                    sheet.Column(4).Width = 34;
                    sheet.Column(5).Width = 34;
                    sheet.Column(6).Width = 12;
                    sheet.Column(7).Width = 12;
                    sheet.Column(8).Width = 12;
                    sheet.Column(9).Width = 20;
                    sheet.Column(10).Width = 18;
                    sheet.Column(11).Width = 18;
                    sheet.Column(12).Width = 18;
                    sheet.Column(13).Width = 18;
                    sheet.Column(14).Width = 18;
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
}
