import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class WebsocketService {

    public connected: boolean;
    private socket: WebSocket;
    public constructor() {

    }

    public connect(): void {
        this.socket = new WebSocket('ws://localhost:3001');

        this.socket.addEventListener('open', () => {
            this.connected = true;
            console.log('连接成功!');
        });

        this.socket.addEventListener('close', () => {
            this.connected = false;
            console.log('断开连接!');
        });

        this.socket.addEventListener('message', event => {
            console.log('接收到消息:', event.data);
        });
    }

    public sendMessage(): void {
        // this.socket.send(fv.message);
    }
}
