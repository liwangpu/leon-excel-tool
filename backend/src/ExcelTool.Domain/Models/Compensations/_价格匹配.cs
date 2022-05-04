using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.Compensations
{

    public class _SKU匹配
    {
        [Column("MSKU")]
        public string MSKU { get; set; }
        [Column("SKU")]
        public string SKU { get; set; }
    }

    public class _价格匹配
    {
        [Column("SKU")]
        public string SKU { get; set; }
        [Column("采购成本单价")]
        public decimal _成本单价 { get; set; }
    }
}
