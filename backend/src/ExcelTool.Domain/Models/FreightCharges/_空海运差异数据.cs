using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.FreightCharges
{
    public class _空海运差异数据
    {
        [Column("DATE发件日期")]
        public string DATE发件日期 { get; set; }
        [Column("CONS运单号")]
        public string CONS运单号 { get; set; }
        [Column("业务员")]
        public string _业务员 { get; set; }
        [Column("组别")]
        public string _组别 { get; set; }
        [Column("头程调拨")]
        public string _头程调拨 { get; set; }
        [Column("二程调拨")]
        public string _二程调拨 { get; set; }
        [Column("应付")]
        public decimal _应付 { get; set; }
        [Column("预估运费")]
        public decimal _预估运费 { get; set; }
        public string group { get; set; }
        public decimal _占比 { get; set; }
        public decimal _实际应付 { get; set; }
    }
}
