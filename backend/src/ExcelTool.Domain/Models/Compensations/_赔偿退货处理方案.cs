using Npoi.Mapper.Attributes;
using System.Collections.Generic;

namespace ExcelTool.Domain.Models.Compensations
{
    public class _赔偿退货处理方案
    {
        [Column("reason")]
        public string reason { get; set; }
        [Column("原因")]
        public string _原因 { get; set; }
        /// <summary>
        /// 状态作为二级筛选
        /// 这个字段是逗号分隔的数组,如果有这个字段,那么这个reason就需要根据二级筛选执行,满足二级才执行这个方案处理
        /// </summary>
        [Column("状态")]
        public string _状态 { get; set; }
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
        public HashSet<string> _状态匹配 { get; set; } = new HashSet<string>();

        public void _数据初始化()
        {
            if (!string.IsNullOrWhiteSpace(_状态))
            {
                var arr = _状态.ToUpper().Split(',');
                foreach (var a in arr)
                {
                    _状态匹配.Add(a);
                }
            }
        }

        public bool _是否符合二级筛选(string status)
        {
            if (string.IsNullOrWhiteSpace(_状态)) { return true; }

            return string.IsNullOrWhiteSpace(status) ? false : _状态匹配.Contains(status);
        }
    }

}
