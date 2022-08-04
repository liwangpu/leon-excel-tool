using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.AmazonShopProfits
{
    public class _亚马逊汇总表各站点标题匹配
    {
        [Column("国家")]
        public string Country { get; set; }
        [Column("date/time")]
        public string DTime { get; set; }
        [Column("settlement id")]
        public string SettlementId { get; set; }
        [Column("type")]
        public string Type { get; set; }
        [Column("order id")]
        public string OrderId { get; set; }
        [Column("sku")]
        public string Sku { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("total")]
        public string Total { get; set; }
    }
}
