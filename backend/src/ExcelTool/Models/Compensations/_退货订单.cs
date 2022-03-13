using ExcelTool.Common;

namespace ExcelTool.Models.Compensations
{
    public class _退货订单
    {
        [ExcelColumn("店铺")]
        public string _店铺 { get; set; }
        [ExcelColumn("国家")]
        public string _国家 { get; set; }
        [ExcelColumn("品名")]
        public string _品名 { get; set; }
        [ExcelColumn("sku")]
        public string SKU { get; set; }
        [ExcelColumn("分类")]
        public string _分类 { get; set; }
        [ExcelColumn("品牌")]
        public string _品牌 { get; set; }
        [ExcelColumn("订单号")]
        public string _订单号 { get; set; }
        [ExcelColumn("商品名称")]
        public string _商品名称 { get; set; }
        [ExcelColumn("MSKU")]
        public string MSKU { get; set; }
        [ExcelColumn("ASIN")]
        public string ASIN { get; set; }
        [ExcelColumn("数量")]
        public int _数量 { get; set; }
        [ExcelColumn("发货仓库编号")]
        public string _发货仓库编号 { get; set; }
        [ExcelColumn("库存属性")]
        public string _库存属性 { get; set; }
        [ExcelColumn("退货原因")]
        public string _退货原因 { get; set; }
        [ExcelColumn("状态")]
        public string _状态 { get; set; }
        [ExcelColumn("LPN编号")]
        public string LPN编号 { get; set; }
        [ExcelColumn("买家备注")]
        public string _买家备注 { get; set; }
        [ExcelColumn("退货时间")]
        public string _退货时间 { get; set; }
        [ExcelColumn("订购时间")]
        public string _订购时间 { get; set; }
        [ExcelColumn("备注")]
        public string _备注 { get; set; }

        public string _部门 { get; set; }
        public bool _需要ERP操作 { get; set; }
        public bool _需要后台操作 { get; set; }
    }
}
