using CsvHelper.Configuration.Attributes;
using System;

namespace ExcelTool.Domain.Models.AliExpress
{
    public class _速卖通支付宝国际账单汇总表
    {
        [Index(0)]
        public string Time { get; set; }

        [Index(1)]
        public string Type { get; set; }
        [Index(2)]
        public string ReferenceNo { get; set; }
        [Index(3)]
        public string AlipayTransactionNo { get; set; }
        [Index(4)]
        public decimal Amount { get; set; }
        [Index(5)]
        public decimal Balance { get; set; }
        [Index(6)]
        public string Currency { get; set; }
        [Index(7)]
        public string Remarks { get; set; }
        [Index(8)]
        public string MerchantTransactionNo { get; set; }
        [Index(9)]
        public string ProductName { get; set; }
        [Index(10)]
        public string CounterpartyName { get; set; }
        [Index(11)]
        public string CounterpartyAccount { get; set; }
        [Index(12)]
        public string ExchangeRate { get; set; }
        [Ignore]
        public string _店铺名称 { get; set; }
    }
}
