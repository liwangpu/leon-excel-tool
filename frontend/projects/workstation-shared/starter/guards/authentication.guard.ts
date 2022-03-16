import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { IUserProfileProvider, USER_PROFILE_PROVIDER } from 'workstation-core';
import { Observable } from 'rxjs';

@Injectable()
export class AuthenticationGuard implements CanActivate {

    private browserMode: boolean = false;
    public constructor(
        @Inject(USER_PROFILE_PROVIDER)
        protected userProvider: IUserProfileProvider,
        @Inject(PLATFORM_ID) private platformId: string,
        private router: Router
    ) {
        this.browserMode = isPlatformBrowser(this.platformId);
    }

    public async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        try {
            await this.userProvider.getProfile();
        } catch (error) {
            this.userProvider.clearProfile();
            return false;
        }
        return true;
    }

}
