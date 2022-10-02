
using ExcelTool.Common;
using ExcelTool.Domain.Handler.Compensations;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;
using ExcelTool.Domain.Handler;

namespace ExcelTool.Commands.AliExpress
{
    public class AlipayIntlBillCollectUploadCommandHandler : IRequestHandler<AlipayIntlBillCollectUploadCommand, MemoryStream>
    {
        private readonly IStaticFileSetting fileSetting;

        public AlipayIntlBillCollectUploadCommandHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public async Task<MemoryStream> Handle(AlipayIntlBillCollectUploadCommand request, CancellationToken cancellationToken)
        {
            var currentTmpFolder = fileSetting.GenerateTemporaryFolder();
            var exportTmpFolder = fileSetting.GenerateTemporaryFolder();
            if (request.Packages.Count > 0)
            {
                foreach (var file in request.Packages)
                {
                    string fileNameWithPath = Path.Combine(currentTmpFolder, file.FileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }
            var handler = new _速卖通支付宝国际账单Handler(currentTmpFolder, exportTmpFolder);
            var ms = await handler.Handle();
            Directory.Delete(currentTmpFolder, true);
            return ms;
        }
    }
}
