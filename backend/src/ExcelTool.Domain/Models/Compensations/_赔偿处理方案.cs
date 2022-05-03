using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.Compensations
{
    public class _赔偿处理方案
    {
        [Column("reason")]
        public string reason { get; set; }
        [Column("原因")]
        public string _原因 { get; set; }
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
