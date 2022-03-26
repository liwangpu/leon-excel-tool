import { Controller, Post, UploadedFiles, UseInterceptors } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { FileFieldsInterceptor } from '@nestjs/platform-express';

@Controller('excel-tool')
export class ExcelToolController {

    public constructor(
        private commandBus: CommandBus
    ) { }

    @Post('upload')
    @UseInterceptors(FileFieldsInterceptor([
        { name: 'af' },
        { name: 'bf' }
    ]))
    public uploadFile(@UploadedFiles() files: { af?: Express.Multer.File[], bf?: Express.Multer.File[]}) {
        console.log('files:', files);

    }
}
