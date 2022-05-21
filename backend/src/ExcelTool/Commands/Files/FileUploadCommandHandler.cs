using ExcelTool.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace ExcelTool.Commands.Files
{
    public class FileUploadCommandHandler : IRequestHandler<FileUploadCommand>
    {
        private readonly IStaticFileSetting fileSetting;

        public FileUploadCommandHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public async Task<Unit> Handle(FileUploadCommand request, CancellationToken cancellationToken)
        {
            if (request.File != null)
            {
                var path = Path.Combine(fileSetting.AttactmentFolder, request.FileKey);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (var targetStream = File.Create(path))
                {
                    await request.File.CopyToAsync(targetStream);
                    targetStream.Close();
                }
            }
            return Unit.Value;
        }
    }
}
