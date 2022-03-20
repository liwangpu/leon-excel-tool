using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace ExcelTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                         .UseKestrel(options =>
                         {
                             options.Limits.MaxRequestBodySize = long.MaxValue;
                         })
                         .UseIISIntegration();
                    webBuilder.UseWebRoot("wwwroot");
#if DEBUG
                    webBuilder.UseUrls("http://*:3101");
#endif
                })
             .UseNLog();
    }
}
