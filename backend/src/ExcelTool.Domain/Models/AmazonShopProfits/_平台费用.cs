using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.AmazonShopProfits
{
    public class _平台费用
    {
        [Column("店铺名称")]
        public string _店铺 { get; set; }
        [Column("组别")]
        public string _组别 { get; set; }
        [Column("运营")]
        public string _运营 { get; set; }
        [Column("7月状态")]
        public string _7月状态 { get; set; }
        [Column("币种")]
        public string _币种 { get; set; }
        [Column("汇率")]
        public decimal _汇率 { get; set; }
        [Column("广告")]
        public decimal _广告 { get; set; }
        [Column("秒杀")]
        public decimal _秒杀 { get; set; }
        [Column("退款")]
        public decimal _退款 { get; set; }
        [Column("长期仓储费")]
        public decimal _长期仓储费 { get; set; }
        [Column("仓储费")]
        public decimal _仓储费 { get; set; }
        [Column("库存放置费")]
        public decimal _库存放置费 { get; set; }
        [Column("移除费")]
        public decimal _移除费 { get; set; }
        [Column("弃置")]
        public decimal _弃置 { get; set; }
        [Column("月租")]
        public decimal _月租 { get; set; }
        [Column("索赔")]
        public decimal _索赔 { get; set; }
        [Column("自报加项")]
        public decimal _自报加项 { get; set; }
        [Column("刷单(美金)")]
        public decimal _刷单_美金 { get; set; }
        [Column("总计")]
        public decimal _总计 { get; set; }
        [Column("月系统汇率")]
        public decimal _月系统汇率 { get; set; }
        [Column("月平均汇率")]
        public decimal _月平均汇率 { get; set; }
        [Column("汇率差")]
        public decimal _汇率差 { get; set; }
        [Column("汇差")]
        public decimal _汇差 { get; set; }
        [Column("备注")]
        public string _备注 { get; set; }
    }
}
