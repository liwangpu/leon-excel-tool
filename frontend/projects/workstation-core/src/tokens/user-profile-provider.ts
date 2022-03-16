import { InjectionToken } from '@angular/core';

export interface IUserProfile {
    identityId?: string;
    employeeId?: string;
    tenantId?: string;
    roleIds?: Array<string>;
    departmentIds?: Array<string>;
    isTenantAdmin?: boolean;
    isPlatformAdmin?: boolean;
    name?: string;
    userAccount?: string;
    avatar?: string;
    [key: string]: any;
}

export interface IUserProfileProvider {
    getProfile(): Promise<IUserProfile>;
    clearProfile(): void;
}

export const USER_PROFILE_PROVIDER: InjectionToken<IUserProfileProvider> = new InjectionToken<IUserProfileProvider>('user profile provider');
