import { IssueTokenCommand } from '@app/commands';
import { JwtAuthGuard, LocalAuthGuard } from '@app/common';
import { Controller, Get, Post, Request, UseGuards } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';


@Controller('auth')
export class AuthController {

    public constructor(
        private commandBus: CommandBus
    ) { }

    @UseGuards(LocalAuthGuard)
    @Post('login')
    public async login(@Request() req): Promise<any> {
        return this.commandBus.execute(new IssueTokenCommand(req.user));
    }

    @UseGuards(JwtAuthGuard)
    @Get('profile')
    public getProfile(@Request() req) {
        return req.user;
    }
}
