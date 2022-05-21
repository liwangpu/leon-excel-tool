using ExcelTool.Common;
using ExcelTool.Domain.Handler.Compensations;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace ExcelTool.Commands.Compensations
{
    public class AmazonCompensationUploadCommandHandler : IRequestHandler<AmazonCompensationUploadCommand, MemoryStream>
    {
        private readonly IStaticFileSetting fileSetting;

        public AmazonCompensationUploadCommandHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public async Task<MemoryStream> Handle(AmazonCompensationUploadCommand request, CancellationToken cancellationToken)
        {
            var currentTmpFolder = fileSetting.GenerateTemporaryFolder();

            var compensationFilePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");
            var storeChangeNameFilePath = Path.Combine(fileSetting.AttactmentFolder, "领星店铺更名为南棠店铺匹配表.xlsx");

            using (var targetStream = File.Create(compensationFilePath))
            {
                await request._赔偿订单.CopyToAsync(targetStream);
                targetStream.Close();
            }

            if (request._店铺更名文件 != null)
            {
                using (var targetStream = File.Create(storeChangeNameFilePath))
                {
                    await request._店铺更名文件.CopyToAsync(targetStream);
                    targetStream.Close();
                }
            }


            var handler = new AmazonCompensationHandler(compensationFilePath, storeChangeNameFilePath, currentTmpFolder);
            var ms = await handler.Handle();
            Directory.Delete(currentTmpFolder, true);
            return ms;
        }
    }
}
