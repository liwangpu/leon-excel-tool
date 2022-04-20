import { CompensationAnalysisCommand } from '@app/commands';
import { JwtAuthGuard, StorageAccessory, User } from '@app/common';
import { SocketService } from '@app/socket';
import { Controller, Param, Post, UploadedFiles, UseGuards, UseInterceptors, Headers } from '@nestjs/common';
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
        private commandBus: CommandBus,
        private socketSrv: SocketService
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
    public compensationUpload(@UploadedFiles() files: { compensations?: Express.Multer.File, refunds?: Express.Multer.File }, @Headers('socketId') socketId) {
        return this.commandBus.execute(new CompensationAnalysisCommand(files, socketId));
    }

    @Post('test-upload')
    @UseInterceptors(FileFieldsInterceptor([
        { name: 'af' }
    ]))
    public uploadFile(@UploadedFiles() files: { af?: Express.Multer.File[] }) {
        console.log('files:', files?.af?.length);

    }

    @UseGuards(JwtAuthGuard)
    @Post('message/:clientId')
    public message(@Param('clientId') clientId) {
        console.log('clientId:', clientId);
        // const writeStream = fs.createWriteStream(path);
        // writeStream.write(file.buffer);
        // writeStream.end();
        // files?.af?.forEach(f=>{

        // });
        // this.socketSrv.socket.serveClient()
        this.socketSrv.socket.to(clientId).emit('chat message', '开心就好');
    }
}
