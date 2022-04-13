import { CompensationAnalysisCommand } from '@app/commands';
import { JwtAuthGuard, StorageAccessory, User } from '@app/common';
import { Controller, Post, UploadedFiles, UseGuards, UseInterceptors } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { FileFieldsInterceptor } from '@nestjs/platform-express';
import { ConnectedSocket } from '@nestjs/websockets';
import { diskStorage } from 'multer';


function customFileName(req, file, cb) {
    const uniqueSuffix = Date.now() + '-' + Math.round(Math.random() * 1e9);
    let fileExtension = "xlsx";

    const originalName = file.originalname.split(".")[0];
    cb(null, originalName + '-' + uniqueSuffix + "." + fileExtension);
}

function destinationPath(req, file, cb) {
    cb(null, './files/')
}

@Controller('excel-tool')
export class ExcelToolController {

    public constructor(
        private commandBus: CommandBus
    ) { }

    /**
     * 退货赔偿订单处理
     * @param files 
     * @returns 
     */
    @UseGuards(JwtAuthGuard)
    @Post('compensation/upload')
    @UseInterceptors(FileFieldsInterceptor([
        { name: 'compensations' },
        { name: 'refunds' }
    ]))
    public compensationUpload(@UploadedFiles() files: { compensations?: Express.Multer.File, refunds?: Express.Multer.File }) {
        // console.log('1:',client);
        return this.commandBus.execute(new CompensationAnalysisCommand(files));
    }

    @Post('test-upload')
    @UseInterceptors(FileFieldsInterceptor([
        { name: 'af' }
    ]))
    public uploadFile(@UploadedFiles() files: { af?: Express.Multer.File[] }) {
        console.log('files:', files?.af?.length);

    }

    @Post('test-upload2')
    @UseInterceptors(FileFieldsInterceptor([
        { name: 'af' }
    ]))
    public uploadFile2(@UploadedFiles() files: { af?: Array<Express.Multer.File> }) {
        console.log('files:', files?.af);
        // const writeStream = fs.createWriteStream(path);
        // writeStream.write(file.buffer);
        // writeStream.end();
        // files?.af?.forEach(f=>{

        // });
    }
}
