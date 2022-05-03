using ExcelTool.Domain.Models.Commons;
using System.Collections.Generic;

namespace ExcelTool.Domain.Utils
{
    public class StoreNameHelper
    {
        protected Dictionary<string, _站点更名匹配表> Maps = new Dictionary<string, _站点更名匹配表>();
        public void LoadData(List<_站点更名匹配表> list)
        {
            list.ForEach(it =>
            {
                Maps.Add(it._国家, it);
            });
        }

        public string _标准化店铺名称(string country, string storeName)
        {
            // 先移除 - 后面的国家
            storeName = storeName.LastIndexOf("-") > -1 ? storeName.Substring(0, storeName.LastIndexOf("-")).Trim() : storeName;
            // 店铺如果结尾没有站的,加上
            if (storeName[storeName.Length - 1] != '站')
            {
                storeName += "站";
            }

            if (Maps.ContainsKey(country))
            {
                var m = Maps[country];
                if (storeName.IndexOf(m._领星店铺名称) > -1)
                {
                    storeName = storeName.Replace(m._领星店铺名称, m._南棠店铺名称);
                }
            }

            return storeName;
        }
    }
}
