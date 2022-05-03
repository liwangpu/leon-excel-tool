using Npoi.Mapper.Attributes;


namespace ExcelTool.Domain.Models.Commons
{
    public class _站点更名匹配表
    {
        [Column("领星店铺名称")]
        public string _领星店铺名称 { get; set; }
        [Column("南棠店铺名称")]
        public string _南棠店铺名称 { get; set; }
        [Column("国家")]
        public string _国家{ get; set; }
    }
}
