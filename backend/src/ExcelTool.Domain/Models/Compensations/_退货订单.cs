using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.Compensations
{
    public class _退货订单
    {
        [Column("店铺")]
        public string _店铺 { get; set; }
        [Column("国家")]
        public string _国家 { get; set; }
        [Column("品名")]
        public string _品名 { get; set; }
        [Column("sku")]
        public string SKU { get; set; }
        [Column("分类")]
        public string _分类 { get; set; }
        [Column("品牌")]
        public string _品牌 { get; set; }
        [Column("订单号")]
        public string _订单号 { get; set; }
        [Column("商品名称")]
        public string _商品名称 { get; set; }
        [Column("MSKU")]
        public string MSKU { get; set; }
        [Column("ASIN")]
        public string ASIN { get; set; }
        [Column("退货数量")]
        public int _数量 { get; set; }
        [Column("发货仓库编号")]
        public string _发货仓库编号 { get; set; }
        [Column("库存属性")]
        public string _库存属性 { get; set; }
        [Column("退货原因")]
        public string _退货原因 { get; set; }
        [Column("状态")]
        public string _状态 { get; set; }
        [Column("LPN编号")]
        public string LPN编号 { get; set; }
        [Column("买家备注")]
        public string _买家备注 { get; set; }
        [Column("退货时间")]
        public string _退货时间 { get; set; }
        [Column("订购时间")]
        public string _订购时间 { get; set; }
        [Column("备注")]
        public string _备注 { get; set; }

        public string _部门 { get; set; }
        public bool _需要ERP操作 { get; set; }
        public bool _需要后台操作 { get; set; }
        public string _对应的ERP操作 { get; set; }
        public string _对应的后台操作 { get; set; }
    }
}
