import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { API_GATEWAY, IdentityStore, ITokenStore, IUserProfile, TOKEN_STORE } from 'workstation-core';
import * as queryString from 'query-string';
import { Observable, tap } from 'rxjs';

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

    public login(username: string, password: string): Observable<any> {
        let url: string = `${this.uri}/auth/login`;
        return this.httpClient.post<any>(url, { username, password }).pipe(tap(res => {
            this.tokenStore.setToken(res.access_token);
        }));
    }

    public getProfile(): Observable<IUserProfile> {
        let url: string = `${this.uri}/auth/profile`;
        return this.httpClient.get<IUserProfile>(url);
    }

}
