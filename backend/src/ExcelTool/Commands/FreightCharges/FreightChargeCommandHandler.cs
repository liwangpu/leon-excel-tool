using ExcelTool.Common;
using ExcelTool.Domain.Handler;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace ExcelTool.Commands.FreightCharges
{
    public class FreightChargeCommandHandler : IRequestHandler<FreightChargeCommand, MemoryStream>
    {
        private readonly IStaticFileSetting fileSetting;

        public FreightChargeCommandHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public async Task<MemoryStream> Handle(FreightChargeCommand request, CancellationToken cancellationToken)
        {
            var currentTmpFolder = fileSetting.GenerateTemporaryFolder();

            var freightChargeFilePath = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");

            using (var targetStream = File.Create(freightChargeFilePath))
            {
                await request._空海运差异比例文件.CopyToAsync(targetStream);
                targetStream.Close();
            }

            var handler = new FreightChargeHandler(freightChargeFilePath, currentTmpFolder);
            var ms = await handler.Handle();
            Directory.Delete(currentTmpFolder, true);
            return ms;
        }
    }
}
