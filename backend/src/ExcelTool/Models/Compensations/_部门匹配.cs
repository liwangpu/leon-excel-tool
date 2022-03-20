using Npoi.Mapper.Attributes;

namespace ExcelTool.Models.Compensations
{
    public class _部门匹配
    {
        [Column("店铺名")]
        public string _店铺名 { get; set; }
        [Column("负责人部门")]
        public string _负责人部门 { get; set; }
        public string _部门 { get; set; }
    }
}
