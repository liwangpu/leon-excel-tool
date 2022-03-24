import { Location } from '@angular/common';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { IdentityStore, IDENTITY_STORE, ITokenStore, TOKEN_STORE } from 'workstation-core';
import { Observable, throwError } from 'rxjs';
import { catchError, share, switchMap, tap } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

    private refreshTokenRequest$: Observable<{ access_token: string; refresh_token: string }>;
    public constructor(
        @Inject(IDENTITY_STORE)
        private identityStore: IdentityStore,
        @Inject(TOKEN_STORE)
        private tokenStore: ITokenStore,
        private router: Router,
        private location: Location
    ) { }

    public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(response => {
            const currentUrl: string = this.location.path();
            const loginUrl: string = currentUrl ? `/login?return=${encodeURIComponent(currentUrl)}` : '/login';
            if (response.status === 401 && typeof location !== 'undefined') {
                this.gotoLogin(loginUrl);
            }
            return throwError(response);
        })) as any;
    }

    private gotoLogin(loginUrl: string): void {
        this.router.navigateByUrl(loginUrl).then().catch();
    }

}
