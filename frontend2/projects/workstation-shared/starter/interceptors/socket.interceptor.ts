import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import * as fromCore from 'workstation-core';
import { Observable } from 'rxjs';

@Injectable()
export class SocketInterceptor implements HttpInterceptor {

    public constructor(
        @Inject(fromCore.SOCKET_CLIENT)
        private socketClient: fromCore.ISocketClient
    ) { }

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (this.socketClient.socket) {
            let secureHeaders: any = req.headers;
            secureHeaders = secureHeaders.append('socketId', this.socketClient.socket.id);
            const secureReq: any = req.clone({ headers: secureHeaders });
            return next.handle(secureReq);
        }
        return next.handle(req);
    }
}
