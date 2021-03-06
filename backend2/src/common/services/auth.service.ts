import { UserService } from '@app/user';
import { Injectable } from '@nestjs/common';
import { JwtService } from '@nestjs/jwt';

@Injectable()
export class AuthService {

    public constructor(
        private usersService: UserService,
        private jwtService: JwtService
    ) {
    }

    public async validateUser(username: string, pass: string): Promise<any> {
        const user = await this.usersService.findOne(username);
        if (user && user.password === pass) {
            const { password, ...result } = user;
            return result;
        }
        return null;
    }

    public async issueToken(user: any) {
        const payload = { name: user.name, sub: user.userId };
        return {
            access_token: this.jwtService.sign(payload),
        };
    }
}
