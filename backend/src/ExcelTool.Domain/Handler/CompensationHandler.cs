using ExcelTool.Domain.Models.Compensations;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExcelTool.Domain.Handler.Compensations
{
    public class CompensationHandler
    {
        protected string path赔偿订单文件;
        protected string path退货订单文件;
        protected string path处理方案文件;
        protected string path部门映射文件;

        #region ctor
        protected CompensationHandler()
        {

        }
        public CompensationHandler(string p赔偿订单文件, string p退货订单文件, string p处理方案文件, string p部门映射文件)
        {
            path赔偿订单文件 = p赔偿订单文件;
            path退货订单文件 = p退货订单文件;
            path处理方案文件 = p处理方案文件;
            path部门映射文件 = p部门映射文件;
        }
        #endregion

        public async Task<MemoryStream> Handle()
        {
            var list退货订单 = new List<_退货订单>();
            var list赔偿订单 = new List<_赔偿订单>();
            var list退货处理方案 = new List<_退货处理方案>();
            var list赔偿处理方案 = new List<_赔偿处理方案>();
            var list部门匹配 = new List<_部门匹配>();
            var list部门 = new List<string>();

            #region 数据读取
            if (!string.IsNullOrEmpty(path赔偿订单文件))
            {
                var mapper = new Mapper(path赔偿订单文件);
                var sheetDatas = mapper.Take<_赔偿订单>().Select(x => x.Value).ToList();
                sheetDatas.ForEach(it =>
                {
                    it._店铺 = it._店铺.Substring(0, it._店铺.LastIndexOf("-")).Trim();
                });
                list赔偿订单.AddRange(sheetDatas);
            }

            if (!string.IsNullOrEmpty(path退货订单文件))
            {
                var mapper = new Mapper(path退货订单文件);
                var sheetDatas = mapper.Take<_退货订单>().Select(x => x.Value).ToList();
                sheetDatas.ForEach(it =>
                {
                    it._店铺 = it._店铺.Substring(0, it._店铺.LastIndexOf("-")).Trim();
                });
                list退货订单.AddRange(sheetDatas);
            }

            if (!string.IsNullOrEmpty(path处理方案文件))
            {
                var mapper = new Mapper(path处理方案文件);
                var data1 = mapper.Take<_赔偿处理方案>("赔偿处理方案").Select(x => x.Value).ToList();
                list赔偿处理方案.AddRange(data1);

                var data2 = mapper.Take<_退货处理方案>("退货处理方案").Select(x => x.Value).ToList();
                list退货处理方案.AddRange(data2);
            }

            if (!string.IsNullOrEmpty(path部门映射文件))
            {
                var mapper = new Mapper(path部门映射文件);
                var sheetDatas = mapper.Take<_部门匹配>().Select(x => x.Value).ToList();
                sheetDatas.ForEach(it =>
                {
                    string pattern = @"[一二三四五六七八九十]{1,2}部";
                    var m = Regex.Match(it._负责人部门, pattern);
                    if (m.Success)
                    {
                        list部门.Add(m.Value);
                        it._部门 = m.Value;
                    }
                    else
                    {
                        it._部门 = "未匹配";
                        list部门.Add("未匹配");
                    }
                });

                list部门匹配.AddRange(sheetDatas);
                // 添加未匹配部门
                list部门.Add("未匹配");
                list部门 = list部门.Select(n => n).Distinct().ToList();
            }
            #endregion

            #region 数据处理
            var list赔偿单需要EPR处理的 = list赔偿处理方案.Where(x => x._需要ERP操作).ToList();
            var list赔偿单需要后台处理的 = list赔偿处理方案.Where(x => x._需要后台操作).ToList();
            var list退货单需要EPR处理的 = list退货处理方案.Where(x => x._需要ERP操作).ToList();
            var list退货单需要后台处理的 = list退货处理方案.Where(x => x._需要后台操作).ToList();

            list赔偿订单.ForEach(data =>
            {
                // 匹配部门
                var dep = list部门匹配.Find(x => x._店铺名 == data._店铺);
                data._部门 = dep != null ? dep._部门 : "未匹配";
                var a1 = list赔偿单需要EPR处理的.FirstOrDefault(x => x.reason == data._原因);
                //data._需要ERP操作 = a1 != null;
                //data._对应的ERP操作 = a1 != null ? a1._ERP执行动作 : null;
                var a2 = list赔偿单需要后台处理的.FirstOrDefault(x => x.reason == data._原因);
                //data._需要后台操作 = a2 != null;
                //data._对应的后台操作 = a2 != null ? a2._后台执行动作 : null;

                if (a1 != null)
                {
                    var item = new _赔偿订单统计项(data, a1._ERP执行动作);
                }


            });

            #endregion

            var aa = 1;
            return null;
        }
    }

    enum enum处理平台
    {
        _ERP,
        _亚马逊后台
    }

    class _赔偿订单统计项
    {

        #region ctor
        private _赔偿订单统计项() { }
        public _赔偿订单统计项(_赔偿订单 data, string operation)
        {
            //_处理平台 = v处理平台;
            _部门 = data._部门;
            _店铺 = data._店铺;
            _单号 = data._订单号;
            MSKU = data.MSKU;
            _处理动作 = operation;
            //_店铺 = v店铺;
            //_单号 = v单号;
            //MSKU = vMSKU;
            //_数量 = v数量;
            //_处理动作 = v处理动作;
            //Key = $"{_处理平台}{_部门}{_店铺}{_处理动作}{MSKU}";
        }
        #endregion

        public string Key { get; protected set; }
        public List<enum处理平台> _处理平台 { get; protected set; }
        public string _部门 { get; protected set; }
        public string _店铺 { get; protected set; }
        public string _单号 { get; protected set; }
        public string MSKU { get; protected set; }
        public string _处理动作 { get; protected set; }
        public HashSet<string> _所有单号 { get; protected set; } = new HashSet<string>();
        public decimal _数量 { get; protected set; }

        public void summary(_赔偿订单统计项 item)
        {
            _所有单号.Add(item._单号);
            _数量 += item._数量;
        }


    }
}
