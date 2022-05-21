using ExcelTool.Common;
using ExcelTool.Domain.Handler.Compensations;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExcelTool.Commands.Compensations
{
    public class CompensationAnalysisUploadCommandHandler : IRequestHandler<CompensationAnalysisUploadCommand, MemoryStream>
    {
        private readonly IStaticFileSetting fileSetting;

        public CompensationAnalysisUploadCommandHandler(IStaticFileSetting fileSetting)
        {
            this.fileSetting = fileSetting;
        }

        public async Task<MemoryStream> Handle(CompensationAnalysisUploadCommand request, CancellationToken cancellationToken)
        {
            var currentTmpFolder = fileSetting.GenerateTemporaryFolder();
            var p赔偿订单 = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");
            var p退货订单 = Path.Combine(currentTmpFolder, $"{Guid.NewGuid().ToString("N")}.xlsx");
            var p部门映射 = Path.Combine(fileSetting.AttactmentFolder, "店铺运营配置表.xlsx");
            var p处理方案 = Path.Combine(fileSetting.AttactmentFolder, "退货赔偿处理方案.xlsx");
            var p店铺更名文件 = Path.Combine(fileSetting.AttactmentFolder, "领星店铺更名为南棠店铺匹配表.xlsx");
            var pMSKU2SKU = Path.Combine(fileSetting.AttactmentFolder, "MSKU_SKU匹配表.xlsx");
            var pSKU价格匹配 = Path.Combine(fileSetting.AttactmentFolder, "SKU_价格匹配表.xlsx");
            var p汇率匹配 = Path.Combine(fileSetting.AttactmentFolder, "汇率匹配表.xlsx");


            if (request._赔偿订单 != null)
            {
                using (var targetStream = File.Create(p赔偿订单))
                {
                    await request._赔偿订单.CopyToAsync(targetStream);
                    targetStream.Close();
                }
            }

            if (request._退货订单 != null)
            {
                using (var targetStream = File.Create(p退货订单))
                {
                    await request._退货订单.CopyToAsync(targetStream);
                    targetStream.Close();
                }
            }

            if (request._部门匹配表 != null)
            {
                using (var targetStream = File.Create(p部门映射))
                {
                    await request._部门匹配表.CopyToAsync(targetStream);
                    targetStream.Close();
                }
            }

            if (request._处理方案 != null)
            {
                using (var targetStream = File.Create(p处理方案))
                {
                    await request._处理方案.CopyToAsync(targetStream);
                    targetStream.Close();
                }
            }

            if (request.MSKU2SKU != null)
            {
                using (var targetStream = File.Create(pMSKU2SKU))
                {
                    await request.MSKU2SKU.CopyToAsync(targetStream);
                    targetStream.Close();
                }
            }

            if (request.SKU2Price != null)
            {
                using (var targetStream = File.Create(pSKU价格匹配))
                {
                    await request.SKU2Price.CopyToAsync(targetStream);
                    targetStream.Close();
                }
            }

            if (request._汇率匹配 != null)
            {
                using (var targetStream = File.Create(p汇率匹配))
                {
                    await request._汇率匹配.CopyToAsync(targetStream);
                    targetStream.Close();
                }
            }

            if (request._店铺更名文件 != null)
            {
                using (var targetStream = File.Create(p店铺更名文件))
                {
                    await request._店铺更名文件.CopyToAsync(targetStream);
                    targetStream.Close();
                }
            }

            var handler = new CompensationHandler(p赔偿订单, p退货订单, p处理方案, p部门映射, p店铺更名文件, pMSKU2SKU, pSKU价格匹配, p汇率匹配, currentTmpFolder);
            var ms = await handler.Handle();
            Directory.Delete(currentTmpFolder, true);
            return ms;
        }
    }

}
