using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.AmazonShopProfits
{
    public class _店铺汇总表
    {
        [Column("国家")]
        public string _国家 { get; set; }
        [Column("类型")]
        public string _类型 { get; set; }
        [Column("描述")]
        public string _描述 { get; set; }
        [Column("交易类型")]
        public string _交易类型 { get; set; }
        [Column("付款详情")]
        public string _付款详情 { get; set; }
        [Column("类别")]
        public string _类别 { get; set; }
    }
}
