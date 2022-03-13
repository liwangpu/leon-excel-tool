using ExcelTool.Common;

namespace ExcelTool.Models.StockAnalysiss
{
    public class _SKU归属
    {
        [ExcelColumn("店铺")]
        public string _店铺 { get; set; }
        [ExcelColumn("SKU")]
        public string SKU { get; set; }
        [ExcelColumn("SKU店铺")]
        public string SKU店铺 { get; set; }
        [ExcelColumn("组别")]
        public string _组别 { get; set; }
        [ExcelColumn("部门")]
        public string _部门 { get; set; }
        [ExcelColumn("运营")]
        public string _运营 { get; set; }
    }
}
