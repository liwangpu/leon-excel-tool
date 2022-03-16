import { Inject, Injectable } from '@angular/core';
import * as fromCore from '@cxist/mirror-workstation-core';
import * as _ from 'lodash';

const tokenStorageKey: string = 'token-info';

@Injectable()
export class TokenStoreService implements fromCore.ITokenStore {

    private tokenInfo: fromCore.ITokenInfo;
    public constructor(
        @Inject(fromCore.APP_MESSAGE_OPSAT)
        private appOpst: fromCore.IMessageOpsat
    ) {
        const str: string = localStorage.getItem(tokenStorageKey);
        if (str) {
            try {
                this.tokenInfo = JSON.parse(str);
            } catch (err) {

            }
        }
    }
    public getToken(): fromCore.ITokenInfo {
        if (this.tokenInfo) { return this.tokenInfo; }
        const str: string = localStorage.getItem(tokenStorageKey);
        if (!str) { return null; }
        return JSON.parse(str);
    }

    public async setToken(info: fromCore.ITokenInfo): Promise<void> {
        if (!info) { return; }
        this.tokenInfo = _.cloneDeep(info);
        const str: string = JSON.stringify(info);
        localStorage.setItem(tokenStorageKey, str);
        this.appOpst.publish('Application:TokenChange', info);
    }

    public clearToken(): void {
        localStorage.removeItem(tokenStorageKey);
        this.tokenInfo = null;
        this.appOpst.publish('Application:ClearToken');
    }
}
