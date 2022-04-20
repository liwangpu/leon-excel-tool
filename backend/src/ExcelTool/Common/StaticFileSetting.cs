using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;

namespace ExcelTool.Common
{
    public class StaticFileSetting : IStaticFileSetting
    {
        public string TmpFolder { get; }
        public StaticFileSetting(IWebHostEnvironment env)
        {
            TmpFolder = Path.Combine(env.WebRootPath, "Tmp");
        }

        public string GenerateTemporaryFolder(string prefix)
        {
            var sid = Guid.NewGuid().ToString("N").ToUpper();
            var tmpFolder = Path.Combine(TmpFolder, string.IsNullOrWhiteSpace(prefix) ? sid : $"{prefix}-{sid}");
            if (!Directory.Exists(tmpFolder))
            {
                Directory.CreateDirectory(tmpFolder);
            }
            return tmpFolder;
        }

        //public string GenerateSpecificFolder(string folderName)
        //{
        //    //var 
        //}

        public string GenerateTemporaryFolder()
        {
            return GenerateTemporaryFolder(null);
        }

    }

    public interface IStaticFileSetting
    {
        string TmpFolder { get; }
        string GenerateTemporaryFolder(string prefix);
        string GenerateTemporaryFolder();
        //string GenerateSpecificFolder(string folderName);
    }
}
