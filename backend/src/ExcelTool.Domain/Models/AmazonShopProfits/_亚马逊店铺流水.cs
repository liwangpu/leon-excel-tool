namespace ExcelTool.Domain.Models.AmazonShopProfits
{
    public class _亚马逊店铺流水
    {
        public string Country { get; set; }
        public string DTime { get; set; }
        public string SettlementId { get; set; }
        public string Type { get; set; }
        public string OrderId { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        //public decimal Total { get; set; }
        public string Total { get; set; }
        public string _店铺 { get; set; }
        public string _交易类型 { get; set; }
        public string _付款详情 { get; set; }
        public string _绩效 { get; set; }
        //public _店铺汇总表 _店铺汇总 { get; set; }

        //public string _交易类型
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(Type))
        //        {
        //            return "优惠";
        //        }
        //        else
        //        {
        //            if (_店铺汇总 == null)
        //            {
        //                return "新类型";
        //            }
        //            else
        //            {
        //                return _店铺汇总._交易类型;
        //            }
        //        }
        //    }
        //}
        //public string _付款详情
        //{
        //    get
        //    {
        //        switch (_交易类型)
        //        {
        //            case "新类型":
        //                return "新类型";
        //            case "订单":
        //                if (!string.IsNullOrWhiteSpace(OrderId) && OrderId.Substring(0, 2).ToUpper() == "S0")
        //                {
        //                    return "代发运费";
        //                }
        //                else
        //                {
        //                    return "订单";
        //                }
        //            default:
        //                if (_店铺汇总 != null)
        //                {
        //                    return _店铺汇总._付款详情;
        //                }
        //                return _交易类型;
        //        }
        //    }
        //}
        //public string _绩效
        //{
        //    get
        //    {
        //        if (_店铺汇总 != null)
        //        {
        //            return _店铺汇总._付款详情;
        //        }

        //        return "新类型";
        //    }
        //}
    }
}
