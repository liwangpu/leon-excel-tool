namespace ExcelTool.Models.EmployedStatistics
{
    /// <summary>
    /// 企业人员情况
    /// </summary>
    public class EmployedStatistic
    {
        public int _年度 { get; set; }
        public string _名称 { get; set; }
        public int _年末从业人员 { get; set; }
        public int _大专以上 { get; set; }
        public int _中高级职称 { get; set; }
        public int _留学归国人员 { get; set; }
        public int _外籍常驻人员 { get; set; }
        public bool _国内公司 { get; set; }
    }
}
