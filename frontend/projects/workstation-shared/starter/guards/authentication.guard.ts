import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import * as fromCore from 'workstation-core';
import { Observable } from 'rxjs';
import { JwtHelperService } from "@auth0/angular-jwt";

@Injectable()
export class AuthenticationGuard implements CanActivate {

    private browserMode: boolean = false;
    private jwtHelper: JwtHelperService;
    public constructor(
        @Inject(fromCore.TOKEN_STORE)
        private tokenStore: fromCore.ITokenStore,
        private router: Router
    ) {
        if (!this.jwtHelper) {
            this.jwtHelper = new JwtHelperService();
        }
    }

    public async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        let tokenInfo = this.tokenStore.getToken();
        let tokenValid = true;

        if (!tokenInfo) {
            tokenValid = false;
        }

        // if(this.jwtHelper.isTokenExpired(tokenInfo)){

        // }
        console.log('token:', tokenInfo);
        return true;
    }

    private gotoLogin(): void {
        this.router.navigateByUrl('/login');
    }

}
