import { InjectionToken } from '@angular/core';
import { Socket } from 'socket.io-client';

export interface ISocketClient {
    get socket(): Socket;
    connect(): void;
    disconnect(): void;
}

export const SOCKET_CLIENT: InjectionToken<ISocketClient> = new InjectionToken<ISocketClient>('socket client');