import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AuthModule } from '@app/auth';
import { UserModule } from '@app/user';
import { MulterModule } from '@nestjs/platform-express';
import { diskStorage } from 'multer';

@Module({
    imports: [
        AuthModule,
        UserModule,
        // MulterModule.register({
        //     dest: './uploads'
        // })
        MulterModule.register({
            storage: diskStorage({
                //自定义路径
                destination: `./fileUpload/`,
                filename: (req, file, cb) => {
                    // 自定义文件名
                    // const filename = `${nuid.next()}.${file.mimetype.split('/')[1]}`;
                    // return cb(null, filename);
                    return cb(null, file.originalname);
                },
            }),
        }),
    ],
    controllers: [AppController],
    providers: [],
})
export class AppModule { }
