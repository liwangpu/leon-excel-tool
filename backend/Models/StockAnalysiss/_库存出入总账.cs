using ExcelTool.Common;

namespace ExcelTool.Models.StockAnalysiss
{
    public class _库存出入总账
    {
        public string _仓库店铺SKU
        {
            get
            {
                return $"{_仓库}{_店铺}{SKU}";
            }
        }
        public int _天数 { get; set; }
        [ExcelColumn("仓库")]
        public string _仓库 { get; set; }
        [ExcelColumn("店铺")]
        public string _店铺 { get; set; }
        [ExcelColumn("SKU")]
        public string SKU { get; set; }
        [ExcelColumn("产品标题")]
        public string _产品标题 { get; set; }
        [ExcelColumn("运营")]
        public string _运营 { get; set; }
        [ExcelColumn("二级部门")]
        public string _二级部门 { get; set; }
        [ExcelColumn("一级部门")]
        public string _一级部门 { get; set; }
        [ExcelColumn("期初数量")]
        public decimal _期初数量 { get; set; }
        [ExcelColumn("期末数量")]
        public decimal _期末数量 { get; set; }
        [ExcelColumn("期初金额")]
        public decimal _期初金额 { get; set; }
        [ExcelColumn("期末金额")]
        public decimal _期末金额 { get; set; }
        [ExcelColumn("入库数量")]
        public decimal _入库数量 { get; set; }
        [ExcelColumn("入库金额")]
        public decimal _入库金额 { get; set; }
        [ExcelColumn("出库数量")]
        public int _出库数量 { get; set; }
        [ExcelColumn("出库金额")]
        public decimal _出库金额 { get; set; }
        [ExcelColumn("销售金额")]
        public decimal _销售金额 { get; set; }
    }
}
