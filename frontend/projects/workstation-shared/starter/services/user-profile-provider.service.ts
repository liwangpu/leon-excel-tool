import { Inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { IdentityStore, IDENTITY_STORE, IUserProfile, IUserProfileProvider } from '@cxist/mirror-workstation-core';
import { Observable } from 'rxjs';
import { share, tap } from 'rxjs/operators';
import { IdentityService } from './identity.service';

@Injectable()
export class UserProfileProviderService implements IUserProfileProvider {

    private profile: { [key: string]: any };
    public constructor(
        @Inject(IDENTITY_STORE)
        private identityStore: IdentityStore
    ) { }

    public async getProfile(): Promise<IUserProfile> {
        if (!this.profile) {
            const res: any = await this.identityStore.getProfile().toPromise();
            this.profile = {
                identityId: res.identityId,
                employeeId: res.employeeId,
                employeeName: res.name,
                userAccount: res.userAccount,
                roleIds: res.roleIds ? res.roleIds.split(',') : [],
                departmentIds: res.departmentIds ? res.departmentIds?.split(',') : [],
                name: res.name,
                tenantId: res.tenantId,
                isTenantAdmin: res.isTenantAdmin,
                isPlatformAdmin: res.isPlatformAdmin,
                avatar: res.avatar

            };
            localStorage.setItem('userProfile', JSON.stringify(this.profile));
        }
        return this.profile;
    }

    public clearProfile(): void {
        this.profile = null;
    }

    private async requestProfile(): Promise<IUserProfile> {

        return null;
    }

}
