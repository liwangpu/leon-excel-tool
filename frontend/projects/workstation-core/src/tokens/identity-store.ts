import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { IUserProfile } from './user-profile-provider';

export interface IdentityStore {
    login(username: string, password: string): Observable<any>;

    getProfile(): Observable<IUserProfile>;

}

export const IDENTITY_STORE: InjectionToken<IdentityStore> = new InjectionToken<IdentityStore>('identity store');
