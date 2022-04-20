import { Injectable, Injector } from '@angular/core';
import { io, Socket } from 'socket.io-client';
import * as fromCore from 'workstation-core';
import * as escapeStringRegexp from 'escape-string-regexp';

@Injectable()
export class SocketClientService implements fromCore.ISocketClient {

    private _socket: Socket;
    @fromCore.LazyService(fromCore.API_GATEWAY)
    private readonly apiGateway: string;

    public constructor(
        protected injector: Injector
    ) { }

    public get socket(): Socket {
        return this._socket;
    }

    public connect(): void {
        const server = this.apiGateway.replace(new RegExp(escapeStringRegexp('http://')), 'ws://');
        this._socket = io(server, { transports: ["websocket"] });
        this.socket.on('connect', () => {
            console.log('服务器连接成功!', this.socket.id);

            // this.socket.on('chat message', msg => {
            //     console.log('mmm:', msg);
            //     //   this.messages.push(msg);
            // });
        });
    }

    public disconnect(): void {

    }
}
