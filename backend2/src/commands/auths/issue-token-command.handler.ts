import { AuthService } from '@app/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { IssueTokenCommand } from './issue-token-command';

@CommandHandler(IssueTokenCommand)
export class IssueTokenCommandHandler implements ICommandHandler<IssueTokenCommand> {

    public constructor(
        private authService: AuthService
    ) {
    }

    public async execute(command: IssueTokenCommand): Promise<any> {
        return this.authService.issueToken(command.user);
    }

}