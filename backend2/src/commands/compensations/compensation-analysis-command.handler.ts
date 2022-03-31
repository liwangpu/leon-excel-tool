import { AuthService } from '@app/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { CompensationAnalysisCommand } from './compensation-analysis-command';


@CommandHandler(CompensationAnalysisCommand)
export class CompensationAnalysisCommandHandler implements ICommandHandler<CompensationAnalysisCommand> {

    public constructor(
    ) {
    }

    public async execute(command: CompensationAnalysisCommand): Promise<any> {
        // return this.authService.issueToken(command.user);


    }

}