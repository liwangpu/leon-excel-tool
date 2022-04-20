import { SocketService } from '@app/socket';
import { Injectable } from '@nestjs/common';
import { MessageBody, SubscribeMessage, WebSocketGateway, WebSocketServer, WsResponse } from '@nestjs/websockets';
import { from, map, Observable } from 'rxjs';
import { Server, Socket } from 'socket.io';

@WebSocketGateway({
    cors: {
        origin: '*',
    }
})
export class EventGateway {
    @WebSocketServer()
    public server: Server;
    public constructor(
        protected socketSrv: SocketService
    ) {
        console.log('ctor:',);
    }

    // private logger: Logger = new Logger('AppGateway');


    afterInit(server: Server) {
        this.socketSrv.socket = server;
    }

    handleDisconnect(client: Socket) {
        //   this.logger.log(`Client disconnected: ${client.id}`);
    }

    handleConnection(client: Socket, ...args: any[]) {
        console.log(`Client connected: ${client.id}`);
    }

    @SubscribeMessage('events')
    findAll(@MessageBody() data: any): Observable<WsResponse<number>> {
        return from([1, 2, 3]).pipe(map(item => ({ event: 'events', data: item })));
    }

    @SubscribeMessage('identity')
    async identity(@MessageBody() data: number): Promise<number> {
        return data;
    }
}
