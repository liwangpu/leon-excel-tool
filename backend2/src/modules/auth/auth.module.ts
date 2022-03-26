import { UserModule } from '@app/user';
import { Module } from '@nestjs/common';
import { AuthService } from '../../common/services/auth.service';
import { LocalAuthGuard } from '../../common/guards/local-auth.guard';
import { LocalStrategy } from '../../common/strategies/local.strategy';
import { PassportModule } from '@nestjs/passport';
import { JwtModule } from '@nestjs/jwt';
import { jwtConstants } from '../../common/consts/constants';
import { JwtStrategy } from '../../common/strategies/jwt.strategy';

@Module({
    imports: [
        UserModule,
        PassportModule,
        JwtModule.register({
            secret: jwtConstants.secret,
            signOptions: { expiresIn: '600s' },
        }),
    ],
    providers: [
        AuthService,
        LocalStrategy,
        JwtStrategy,
        LocalAuthGuard
    ],
    exports: [
        AuthService,
        LocalStrategy,
        LocalAuthGuard
    ]
})
export class AuthModule { }
