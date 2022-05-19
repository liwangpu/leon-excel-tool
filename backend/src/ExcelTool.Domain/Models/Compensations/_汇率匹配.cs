using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.Compensations
{
    public class _汇率匹配
    {
        [Column("币种代码")]
        public string _币种代码 { get; set; }
        [Column("币种名称")]
        public string _币种名称 { get; set; }
        [Column("汇率")]
        public decimal _汇率 { get; set; }
    }
}
