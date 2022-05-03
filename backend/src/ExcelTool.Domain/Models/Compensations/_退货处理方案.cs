using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.Compensations
{
    public class _退货处理方案
    {
        [Column("detailed-disposition")]
        public string DetailedDisposition { get; set; }
        [Column("库存属性")]
        public string _库存属性 { get; set; }
        [Column("是否需要后台操作")]
        public string _是否需要后台操作 { get; set; }
        [Column("是否需要ERP操作")]
        public string _是否需要ERP操作 { get; set; }
        [Column("后台执行动作")]
        public string _后台执行动作 { get; set; }
        [Column("ERP执行动作")]
        public string _ERP执行动作 { get; set; }

        public bool _需要后台操作 { get { return _是否需要后台操作 == "是"; } }
        public bool _需要ERP操作 { get { return _是否需要ERP操作 == "是"; } }
    }
}
