using Npoi.Mapper.Attributes;

namespace ExcelTool.Models.StockAnalysiss
{
    public class _库存差异源数据
    {
        [Column("部门")]
        public string _部门 { get; set; }
        [Column("组别")]
        public string _组别 { get; set; }
        [Column("运营")]
        public string _运营 { get; set; }
        [Column("SKU")]
        public string SKU { get; set; }
        [Column("店铺")]
        public string _店铺 { get; set; }
        [Column("差异数量")]
        public int _差异数量 { get; set; }
        [Column("差异总额")]
        public decimal _差异总额 { get; set; }
        // 附属列
        [Column("ASIN")]
        public string ASIN { get; set; }
        [Column("MSKU")]
        public string MSKU { get; set; }
        [Column("FNSKU")]
        public string FNSKU { get; set; }
        [Column("中文品名")]
        public string _中文品名 { get; set; }
        [Column("预留数量")]
        public int _预留数量 { get; set; }
        [Column("后台可售")]
        public int _后台可售 { get; set; }
        [Column("新系统可用库存")]
        public int _新系统可用库存 { get; set; }
        [Column("新系统库存数量")]
        public int _新系统库存数量 { get; set; }
        [Column("订单占用库存")]
        public int _订单占用库存 { get; set; }
        [Column("签收未入库")]
        public int _签收未入库 { get; set; }
        [Column("后台不可售")]
        public int _后台不可售 { get; set; }
        [Column("成本单价")]
        public decimal _成本单价 { get; set; }
        [Column("头程单价")]
        public decimal _头程单价 { get; set; }
        [Column("差异成本")]
        public decimal _差异成本 { get; set; }
        [Column("差异头程")]
        public decimal _差异头程 { get; set; }
    }
}
