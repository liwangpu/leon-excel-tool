namespace ExcelTool.Domain.Models.Compensations
{
    public abstract class 赔偿退货订单源数据
    {
        public abstract string _领星店铺名称 { get; set; }
        public abstract string _原因 { get; set; }
        public abstract string MSKU { get; set; }
        public string _南棠店铺名称 { get; set; }
        public string _部门 { get; set; }
        public string Key { get; protected set; }
        public abstract void _数据处理();
    }
}
