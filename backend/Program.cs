using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseWebRoot("wwwroot");
#if DEBUG
                    webBuilder.UseUrls("http://*:9001");
#endif
                });
    }
}
