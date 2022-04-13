export class CompensationAnalysisCommand {

    public constructor(
        public readonly files: { compensations?: Express.Multer.File, refunds?: Express.Multer.File }
    ) { }

}