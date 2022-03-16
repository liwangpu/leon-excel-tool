import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { API_GATEWAY, IdentityStore, ITokenInfo, ITokenStore, IUserProfile, TOKEN_STORE } from 'workstation-core';
import * as queryString from 'query-string';
import { Observable } from 'rxjs';

@Injectable()
export class IdentityService implements IdentityStore {

    private uri: string = `${this.gateway}`;
    public constructor(
        @Inject(API_GATEWAY)
        private gateway: string,
        @Inject(TOKEN_STORE)
        private tokenStore: ITokenStore,
        private httpClient: HttpClient
    ) { }

    public login(username: string, password: string, captchaCode?: string): Observable<ITokenInfo> {
        let url: string = `${this.uri}/ids/connect/token`;
        const body: FormData = new FormData();
        body.set('grant_type', 'password');
        body.set('client_id', 'server');
        body.set('username', username);
        body.set('password', password);
        if (captchaCode) {
            body.set('captchaCode', captchaCode);
        }
        return this.httpClient.post<ITokenInfo>(url, body);
    }

    public getProfile(): Observable<IUserProfile> {
        let url: string = `${this.uri}/ids/Identity/Profile`;
        return this.httpClient.get<IUserProfile>(url);
    }

    public queryTenantList(queryParam: any): Observable<Array<any>> {
        const queryPart: string = queryString.stringify(queryParam);
        return this.httpClient.get<any>(`${this.uri}/ids/Identity/tenant?${queryPart}`);
    }
}
