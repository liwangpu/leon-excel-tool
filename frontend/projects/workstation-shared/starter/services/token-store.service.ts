import { Inject, Injectable } from '@angular/core';
import * as fromCore from 'workstation-core';
import * as _ from 'lodash';

const tokenStorageKey: string = 'token-info';

@Injectable()
export class TokenStoreService implements fromCore.ITokenStore {

    private token: string;
    public constructor(
    ) {
        const str: string = localStorage.getItem(tokenStorageKey);
        if (str) {
            this.token = JSON.parse(str);
        }
    }
    public getToken(): string {
        if (this.token) { return this.token; }
        const str: string = localStorage.getItem(tokenStorageKey);
        if (!str) { return null; }
        return JSON.parse(str);
    }

    public async setToken(token: string): Promise<void> {
        if (!token) { return; }
        this.token = token;
        localStorage.setItem(tokenStorageKey, token);
    }

    public clearToken(): void {
        localStorage.removeItem(tokenStorageKey);
        this.token = null;
    }
}
