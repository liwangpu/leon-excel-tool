using ExcelTool.Common;
using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExcelTool.Queries.Files
{
    public class FileExistCheckQueryHandler : IRequestHandler<FileExistCheckQuery, bool>
    {
        private readonly IStaticFileSetting fileSetting;

        public FileExistCheckQueryHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public async Task<bool> Handle(FileExistCheckQuery request, CancellationToken cancellationToken)
        {
            var path = Path.Combine(fileSetting.AttactmentFolder, request.FileKey);
            return File.Exists(path);
        }
    }
}
