import { Inject, Injectable } from '@angular/core';
import * as fromCore from 'workstation-core';
import * as _ from 'lodash';

const tokenStorageKey: string = 'token-info';

@Injectable()
export class TokenStoreService implements fromCore.ITokenStore {

    private token: string;
    public constructor(
    ) {
        this.token = localStorage.getItem(tokenStorageKey);
    }

    public getToken(): string {
        if (this.token) { return this.token; }
        const str: string = localStorage.getItem(tokenStorageKey);
        return str;
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
