using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.AmazonCompensations
{
    public class _赔偿报表源数据
    {
        [Column("店铺")]
        public string _领星店铺名称 { get; set; }
        [Column("国家")]
        public string _国家 { get; set; }
        [Column("时间")]
        public string _时间 { get; set; }
        [Column("赔偿编号")]
        public string _赔偿编号 { get; set; }
        [Column("订单号")]
        public string _订单号 { get; set; }
        [Column("原因")]
        public string _原因 { get; set; }
        [Column("MSKU")]
        public string MSKU { get; set; }
        [Column("ASIN")]
        public string ASIN { get; set; }
        [Column("FNSKU")]
        public string FNSKU { get; set; }
        [Column("标题")]
        public string _标题 { get; set; }
        [Column("状况")]
        public string _状况 { get; set; }
        [Column("币种")]
        public string _币种 { get; set; }

        [Column("每件商品赔偿金额")]
        public decimal _每件商品赔偿金额 { get; set; }
        [Column("总金额")]
        public decimal _总金额 { get; set; }
        [Column("赔偿数量（现金）")]
        public decimal _赔偿数量_现金 { get; set; }
        [Column("赔偿数量（库存）")]
        public decimal _赔偿数量_库存 { get; set; }
        [Column("赔偿数量（总计）")]
        public decimal _赔偿数量_总计 { get; set; }

        [Column("原始赔偿编号")]
        public string _原始赔偿编号 { get; set; }
        [Column("原始赔偿类型")]
        public string _原始赔偿类型 { get; set; }
        public string Key { get; protected set; }
        /// <summary>
        /// 南棠店铺名称
        /// </summary>
        public string _店铺 { get; set; }
        public void _数据处理()
        {
            Key = $"{_店铺}{_赔偿编号}{MSKU}";
        }
    }
}
