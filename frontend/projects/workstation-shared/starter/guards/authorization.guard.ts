import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChild, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable()
export class AuthorizationGuard implements CanActivateChild {

    private browserMode: boolean = false;
    public constructor(
        @Inject(PLATFORM_ID) private platformId: string
    ) {
        this.browserMode = isPlatformBrowser(this.platformId);
    }

    public canActivateChild(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {
        // const redirect: (valid: boolean) => boolean = (valid: boolean) => {
        //     // angular ssr模式,让页面预渲染,把授权细节推迟到浏览器
        //     if (!this.browserMode) {
        //         return true;
        //     } else {
        //         if (!valid) {
        //             this.authorizationPolicy.unAuthorizedRoutingHop();
        //         }
        //     }
        //     return valid;
        // };
        // return this.authorizationPolicy.authorize(next.data.permissionKey).then(redirect);

        return true;
    }

}
