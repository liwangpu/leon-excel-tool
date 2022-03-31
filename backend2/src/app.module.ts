import { Module } from '@nestjs/common';
import { AppController } from './controllers/app.controller';
import { MulterModule } from '@nestjs/platform-express';
import { diskStorage } from 'multer';
import { AuthController } from './controllers/auth.controller';
import { ExcelToolController } from './controllers/excel-tool.controller';
import { UserController } from './controllers/user.controller';
import { UserModule } from '@app/user';
import { ExcelToolModule } from '@app/excel-tool';
import * as fromCommon from '@app/common';
import { PassportModule } from '@nestjs/passport';
import { JwtModule } from '@nestjs/jwt';
import { jwtConstants } from '@app/common';
import { CqrsModule } from '@nestjs/cqrs';
import { CommandHandlers } from '@app/commands';
import { WsStartGateway } from './ws.gateway';

@Module({
    imports: [
        CqrsModule,
        MulterModule.register({
            dest: './uploads'
        }),
        PassportModule,
        JwtModule.register({
            secret: jwtConstants.secret,
            signOptions: { expiresIn: `${60 * 60 * 24}s` },
        }),
        UserModule,
        ExcelToolModule
    ],
    controllers: [
        AppController,
        AuthController,
        ExcelToolController,
        UserController
    ],
    providers: [
        ...CommandHandlers,
        WsStartGateway,
        fromCommon.AuthService,
        fromCommon.LocalStrategy,
        fromCommon.JwtStrategy,
        fromCommon.LocalAuthGuard
    ],
})
export class AppModule { }
