namespace ExcelTool.Domain.Models.Compensations
{
    public abstract class 赔偿退货订单源数据
    {
        public abstract string _领星店铺名称 { get; set; }
        public abstract string _原因 { get; set; }
        public abstract string MSKU { get; set; }
        public string _南棠店铺名称 { get; set; }
        public string _部门 { get; set; }
        public _赔偿退货处理方案 _处理方案 { get; protected set; }
        public string Key { get; protected set; }
        public string _后台执行动作
        {
            get
            {
                if (_处理方案 == null)
                { return null; }
                return _处理方案._后台执行动作;
            }
        }
        public string _ERP执行动作
        {
            get
            {
                if (_处理方案 == null)
                { return null; }
                return _处理方案._ERP执行动作;
            }
        }
        public bool _需要后台操作
        {
            get
            {
                if (_处理方案 == null)
                { return false; }
                return _处理方案._需要后台操作;
            }
        }
        public bool _需要ERP操作
        {
            get
            {
                if (_处理方案 == null)
                { return false; }
                return _处理方案._需要ERP操作;
            }
        }
        public abstract void _数据处理(_赔偿退货处理方案 solution);
    }
}
