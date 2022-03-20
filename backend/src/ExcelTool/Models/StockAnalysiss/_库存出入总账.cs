using Npoi.Mapper.Attributes;
using System.Text.RegularExpressions;

namespace ExcelTool.Models.StockAnalysiss
{
    public class _库存出入总账
    {
        public string _仓库店铺SKU { get; set; }
        public int _天数 { get; set; }
        [Column("仓库")]
        public string _仓库 { get; set; }
        [Column("店铺")]
        public string _店铺 { get; set; }
        [Column("SKU")]
        public string SKU { get; set; }
        [Column("产品标题")]
        public string _产品标题 { get; set; }
        [Column("运营")]
        public string _运营 { get; set; }
        [Column("二级部门")]
        public string _二级部门 { get; set; }
        [Column("一级部门")]
        public string _一级部门 { get; set; }
        [Column("期初数量")]
        public int _期初数量 { get; set; }
        [Column("期末数量")]
        public int _期末数量 { get; set; }
        [Column("期初金额")]
        public double _期初金额 { get; set; }
        [Column("期末金额")]
        public double _期末金额 { get; set; }
        [Column("入库数量")]
        public int _入库数量 { get; set; }
        [Column("入库金额")]
        public double _入库金额 { get; set; }
        [Column("出库数量")]
        public int _出库数量 { get; set; }
        [Column("出库金额")]
        public double _出库金额 { get; set; }
        public bool _退换或弃置 { get; set; }

        public enum仓库类型? _仓库类型 { get; set; }

        public void InitData()
        {
            _仓库店铺SKU = $"{_仓库}{_店铺}{SKU}";

            string p1 = @"退换|弃置";
            _退换或弃置 = _仓库 != null ? Regex.IsMatch(_仓库, p1) : false;
            //店铺含有在途 且仓库 没有在途两个字的 要 在仓库末尾加在途
            if (_店铺 != null && Regex.IsMatch(_店铺, @"在途"))
            {
                _仓库 = $"{_仓库 }在途";
            }
        }

        public void _校验仓库类型()
        {
            var str店铺名称 = string.IsNullOrEmpty(_店铺) ? null : _店铺.ToLower();
            if (str店铺名称 != null && Regex.IsMatch(str店铺名称, @"lazada"))
            {
                _仓库类型 = enum仓库类型.耗材仓;
            }
            else
            {
                if (!string.IsNullOrEmpty(_仓库))
                {
                    if (Regex.IsMatch(_仓库, @"中转"))
                    {
                        _仓库类型 = enum仓库类型.中转仓;
                    }
                    else if (Regex.IsMatch(_仓库, @"虚拟") || Regex.IsMatch(_仓库, @"海外"))
                    {
                        _仓库类型 = enum仓库类型.虚拟仓海外仓;
                    }
                    else if (Regex.IsMatch(_仓库, @"直发"))
                    {
                        _仓库类型 = enum仓库类型.直发仓;
                    }
                    else if (Regex.IsMatch(_仓库, @"耗材"))
                    {
                        _仓库类型 = enum仓库类型.耗材仓;
                    }
                    else
                    {
                        _仓库类型 = enum仓库类型.不明确;
                    }
                }
                else
                {
                    _仓库类型 = enum仓库类型.不明确;
                }
            }
        }
    }

    public enum enum仓库类型
    {
        中转仓,
        虚拟仓海外仓,
        直发仓,
        耗材仓,
        不明确
    }
}
