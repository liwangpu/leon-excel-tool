using System;
using System.IO;

namespace ExcelTool.Domain.Utils
{
    public static class FolderHelper
    {
        public static string GenerateTemporaryFolder(string folderBase)
        {
            var sid = Guid.NewGuid().ToString("N").ToUpper();
            var tmpFolder = Path.Combine(folderBase, sid );
            if (!Directory.Exists(tmpFolder))
            {
                Directory.CreateDirectory(tmpFolder);
            }
            return tmpFolder;
        }
    }
}
