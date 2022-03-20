using Npoi.Mapper.Attributes;
using System;

namespace ExcelTool.Models.StockAnalysiss
{
    public class _销售流水
    {
        [Column("仓库店铺SKU")]
        public string _仓库店铺SKU { get; set; }
        [Column("销售金额 ")]
        public double _销售金额 { get; set; }
        public double _30天销售金额 { get; set; }
        public double _15天销售金额 { get; set; }
        public double _5天销售金额 { get; set; }
        public double _本期平均销售金额 { get; set; }
        public void _计算平均销售金额()
        {
            _30天销售金额 = Math.Round(_销售金额 / 30 * 0.2, 2);
            _15天销售金额 = Math.Round(_销售金额 / 15 * 0.3, 2);
            _5天销售金额 = Math.Round(_销售金额 / 5 * 0.5, 2);
            _本期平均销售金额 = Math.Round((_30天销售金额 + _15天销售金额 + _5天销售金额) * 30, 2);
        }
    }
}
