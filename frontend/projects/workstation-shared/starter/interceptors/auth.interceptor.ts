import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ITokenInfo, ITokenStore, TOKEN_STORE } from 'workstation-core';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    public constructor(
        @Inject(TOKEN_STORE)
        private tokenStore: ITokenStore
    ) { }

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const tokenInfo: ITokenInfo = this.tokenStore.getToken();
        if (tokenInfo) {
            let secureHeaders: any = req.headers;
            secureHeaders = secureHeaders.append('Authorization', `bearer ${tokenInfo.access_token}`);
            const secureReq: any = req.clone({ headers: secureHeaders });
            return next.handle(secureReq);
        }
        return next.handle(req);
    }
}
