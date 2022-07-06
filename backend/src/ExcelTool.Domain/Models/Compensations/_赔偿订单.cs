using Npoi.Mapper.Attributes;
using System;

namespace ExcelTool.Domain.Models.Compensations
{
    public class _赔偿订单 : 赔偿退货订单源数据
    {
        [Column("店铺")]
        public override string _领星店铺名称 { get; set; }
        [Column("国家")]
        public string _国家 { get; set; }
        [Column("时间")]
        public DateTime _时间 { get; set; }
        [Column("赔偿编号")]
        public string _赔偿编号 { get; set; }
        [Column("订单号")]
        public string _订单号 { get; set; }
        [Column("原因")]
        public override string _原因 { get; set; }
        [Column("MSKU")]
        public override string MSKU { get; set; }
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
        public int _赔偿数量_库存 { get; set; }
        [Column("赔偿数量（总计）")]
        public int _赔偿数量_总计 { get; set; }

        [Column("原始赔偿编号")]
        public string _原始赔偿编号 { get; set; }
        [Column("原始赔偿类型")]
        public string _原始赔偿类型 { get; set; }
        public decimal _汇率 { get; set; }
        public override void _数据处理(_赔偿退货处理方案 solution)
        {
            _处理方案 = solution;
            _无匹配SKU = string.IsNullOrWhiteSpace(SKU);
            if (_无匹配SKU)
            {
                SKU = Guid.NewGuid().ToString("N").ToUpper();
            }
            Key = $"{_南棠店铺名称}{SKU}{_原因}";
        }
    }
}
