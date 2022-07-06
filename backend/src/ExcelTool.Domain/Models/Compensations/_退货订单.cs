using Npoi.Mapper.Attributes;
using System;

namespace ExcelTool.Domain.Models.Compensations
{
    public class _退货订单 : 赔偿退货订单源数据
    {
        [Column("店铺")]
        public override string _领星店铺名称 { get; set; }
        [Column("国家")]
        public string _国家 { get; set; }
        [Column("品名")]
        public string _品名 { get; set; }
        [Column("分类")]
        public string _分类 { get; set; }
        [Column("品牌")]
        public string _品牌 { get; set; }
        [Column("订单号")]
        public string _订单号 { get; set; }
        [Column("商品名称")]
        public string _商品名称 { get; set; }
        [Column("MSKU")]
        public override string MSKU { get; set; }
        [Column("ASIN")]
        public string ASIN { get; set; }
        [Column("退货数量")]
        public int _数量 { get; set; }
        [Column("发货仓库编号")]
        public string _发货仓库编号 { get; set; }
        [Column("库存属性")]
        public override string _原因 { get; set; }
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
        public string _状态_原始 { get; set; }
        public override void _数据处理(_赔偿退货处理方案 solution)
        {
            _处理方案 = solution;
            _状态_原始 = _状态;
            _状态 = string.IsNullOrWhiteSpace(_状态) ? null : _状态.ToUpper();
            _无匹配SKU = string.IsNullOrWhiteSpace(SKU);
            if (_无匹配SKU)
            {
                SKU = Guid.NewGuid().ToString("N").ToUpper();
            }
            Key = $"{_南棠店铺名称}{SKU}{_原因}";

        }
    }
}
