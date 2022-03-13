using ExcelTool.Common;

namespace ExcelTool.Models.Compensations
{
    public class _部门匹配
    {
        [ExcelColumn("店铺名")]
        public string _店铺名 { get; set; }
        [ExcelColumn("负责人部门")]
        public string _负责人部门 { get; set; }
        public string _部门 { get; set; }
    }
}
