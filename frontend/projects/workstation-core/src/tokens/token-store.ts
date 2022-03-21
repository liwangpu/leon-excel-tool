import { InjectionToken } from '@angular/core';


export interface ITokenStore {
    getToken(): string;
    setToken(token: string): Promise<void>;
    clearToken(): void;
}

export const TOKEN_STORE: InjectionToken<ITokenStore> = new InjectionToken<ITokenStore>('token store');
