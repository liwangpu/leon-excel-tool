import { InjectionToken } from '@angular/core';

export interface ITokenInfo {
    token: string;
    [key: string]: any;
}

export interface ITokenStore {
    getToken(): ITokenInfo;
    setToken(info: ITokenInfo): Promise<void>;
    clearToken(): void;
}

export const TOKEN_STORE: InjectionToken<ITokenStore> = new InjectionToken<ITokenStore>('token store');
