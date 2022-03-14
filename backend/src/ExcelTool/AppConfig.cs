namespace ExcelTool
{
    public class AppConfig
    {
        public JwtSetting JwtSetting { get; set; }
    }

    public class JwtSetting
    {
        /// <summary>
        /// 发行者
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 受众
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public int AccessExpiration { get; set; }
        /// <summary>
        /// 刷新时间
        /// </summary>
        public int RefreshExpiration { get; set; }
    }
}
