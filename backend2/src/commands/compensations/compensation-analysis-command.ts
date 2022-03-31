export class CompensationAnalysisCommand {

    public constructor(
        public readonly files: { compensations?: Array<Express.Multer.File>, refunds?: Array<Express.Multer.File> }
    ) { }

}