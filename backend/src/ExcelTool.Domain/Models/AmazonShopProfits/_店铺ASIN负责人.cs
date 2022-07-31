using Npoi.Mapper.Attributes;

namespace ExcelTool.Domain.Models.AmazonShopProfits
{
    public class _店铺ASIN负责人
    {
        [Column("店铺")]
        public string _店铺 { get; set; }
        [Column("组别")]
        public string _组别 { get; set; }
        [Column("币种")]
        public string _币种 { get; set; }
        [Column("ASIN负责人")]
        public string ASIN负责人 { get; set; }
        [Column("销售总额RMB")]
        public decimal _销售总额_RMB { get; set; }
        [Column("订单汇率")]
        public decimal _订单汇率 { get; set; }
        [Column("扣除费用所得(原币)")]
        public decimal _扣除费用所得_原币 { get; set; }
        [Column("扣除费用所得(RMB)")]
        public decimal _扣除费用所得_RMB { get; set; }
        [Column("商品成本RMB")]
        public decimal _商品成本RMB { get; set; }
        [Column("头程运费")]
        public decimal _头程运费 { get; set; }
        [Column("包装成本RMB")]
        public decimal _包装成本_RMB { get; set; }
        [Column("快递费")]
        public decimal _快递费 { get; set; }
        [Column("订单利润RMB")]
        public decimal _订单利润_RMB { get; set; }

        public decimal _销售额比例 { get; set; }

        public _平台费用 _平台费用Data { get; set; }
        public decimal _美元汇率
        {
            get
            {
                if (_平台费用Data == null)
                {
                    return 0;
                }

                return _平台费用Data._月系统汇率;
            }
        }
        public decimal _销售总额_美元
        {
            get
            {
                if (_美元汇率 <= 0)
                {
                    return 0;
                }
                return _销售总额_RMB / _美元汇率;
            }
        }
        public decimal _扣除费用所得_美元
        {
            get
            {
                if (_美元汇率 <= 0)
                {
                    return 0;
                }
                return _扣除费用所得_RMB / _美元汇率;
            }
        }
        public decimal _美金利润_美元
        {
            get
            {
                if (_美元汇率 <= 0)
                {
                    return 0;
                }
                return _订单利润_RMB / _美元汇率;
            }
        }
        public decimal _利润率
        {
            get
            {
                if (_销售总额_美元 <= 0)
                {
                    return 0;
                }
                return _美金利润_美元 / _销售总额_美元;
            }
        }
        public decimal _广告
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._广告 * _销售额比例;
            }
        }
        public decimal _秒杀
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._秒杀 * _销售额比例;
            }
        }
        public decimal _退款
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._退款 * _销售额比例;
            }
        }
        public decimal _长期仓储费
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._长期仓储费 * _销售额比例;
            }
        }
        public decimal _仓储费
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._仓储费 * _销售额比例;
            }
        }
        public decimal _库存放置费
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._库存放置费 * _销售额比例;
            }
        }
        public decimal _移除费
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._移除费 * _销售额比例;
            }
        }
        public decimal _弃置
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._弃置 * _销售额比例;
            }
        }

        public decimal _月租
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._月租 * _销售额比例;
            }
        }
        public decimal _索赔
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._索赔 * _销售额比例;
            }
        }
        public decimal _自报加项
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._自报加项 * _销售额比例;
            }
        }
        public decimal _刷单_美金
        {
            get
            {
                if (_销售额比例 <= 0 || _平台费用Data == null)
                {
                    return 0;
                }
                return _平台费用Data._刷单_美金 * _销售额比例;
            }
        }

        public decimal _费用合计
        {
            get
            {
                //return _广告+ _秒杀+ _退款+ _长期仓储费+ _库存放置费+
                return 0;
            }
        }
        public decimal _店铺毛利率
        {
            get
            {
                return 0;
            }
        }
    }
}
