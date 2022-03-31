// auths
import { IssueTokenCommandHandler } from './auths/issue-token-command.handler';
export * from './auths/issue-token-command';
export * from './auths/issue-token-command.handler';
// compensations
import { CompensationAnalysisCommandHandler } from './compensations/compensation-analysis-command.handler';
export * from './compensations/compensation-analysis-command';
export * from './compensations/compensation-analysis-command.handler';

export const CommandHandlers = [
    IssueTokenCommandHandler,
    CompensationAnalysisCommandHandler,
];