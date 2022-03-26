import { Controller, Get, Post, UseGuards, Request, Body, UseInterceptors, UploadedFiles } from '@nestjs/common';
import { AuthGuard } from '@nestjs/passport';
import { FileFieldsInterceptor } from '@nestjs/platform-express';
import { diskStorage } from 'multer';
import { extname } from 'path';

// export const editFileName = (req, file, callback) => {
//     const name = file.originalname.split('.')[0];
//     const fileExtName = extname(file.originalname);
//     const randomName = Array(4)
//         .fill(null)
//         .map(() => Math.round(Math.random() * 16).toString(16))
//         .join('');
//     callback(null, `${name}-${randomName}${fileExtName}`);
// };

@Controller()
export class AppController {
    // public constructor(
    //     private authService: AuthService
    // ) { }

    // @Get()
    // public getHello(): string {
    //     return "hello";
    // }

    // @UseGuards(LocalAuthGuard)
    // @Post('auth/login')
    // public async login(@Request() req): Promise<any> {
    //     return this.authService.login(req.user);
    // }

    // @UseGuards(JwtAuthGuard)
    // @Get('profile')
    // public getProfile(@Request() req) {
    //     return req.user;
    // }

    // @Post('card')
    // public createCard(@Body() card: any) {
    //     console.log('title:', card);
    //     return true;
    // }

    // @Post('upload')
    // @UseInterceptors(FileFieldsInterceptor([
    //     { name: 'af' },
    //     { name: 'bf' },
    //     { name: 'title' },
    // ]))
    // public uploadFile(@Body() dto: any, @UploadedFiles() files: { af?: Express.Multer.File[], bf?: Express.Multer.File[], title: string }) {
    //     console.log('files:', files);
    //     console.log('dto:', dto);
    // }
}
