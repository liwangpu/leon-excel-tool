import { Inject, Injectable } from '@angular/core';
import { Router, Resolve, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { IUserProfileProvider, USER_PROFILE_PROVIDER } from 'workstation-core';

@Injectable()
export class ProfileResolver implements Resolve<boolean> {
    public constructor(
        @Inject(USER_PROFILE_PROVIDER)
        private profileSrv: IUserProfileProvider
    ) {

    }

    public resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<any> {
        try {
            const profile = this.profileSrv.getProfile();
            return profile;
        } catch (err) {
            return null;
        }
    }
}
