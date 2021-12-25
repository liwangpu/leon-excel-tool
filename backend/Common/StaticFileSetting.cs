using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ExcelTool.Common
{
    public class StaticFileSetting : IStaticFileSetting
    {
        public string TmpFolder { get; }
        public StaticFileSetting(IWebHostEnvironment env)
        {
            TmpFolder = Path.Combine(env.WebRootPath, "Tmp");
        }
    }

    public interface IStaticFileSetting
    {
        string TmpFolder { get; }
    }
}
