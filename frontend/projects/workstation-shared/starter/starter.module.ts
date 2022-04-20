import { APP_INITIALIZER, LOCALE_ID, ModuleWithProviders, NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './components/login/login.component';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NZ_CONFIG } from 'ng-zorro-antd/core/config';
import { NzConfig } from 'ng-zorro-antd/core/config';
import { NZ_I18N, zh_CN } from 'ng-zorro-antd/i18n';
import { NzIconService } from 'ng-zorro-antd/icon';
import { LayoutModule } from 'workstation-shared/layout';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { EnvStoreService } from './services/env-store.service';
import * as fromCore from 'workstation-core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { SocketInterceptor } from './interceptors/socket.interceptor';
import { AuthenticationGuard } from './guards/authentication.guard';
import { AuthorizationGuard } from './guards/authorization.guard';
import { IdentityService } from './services/identity.service';
import { OperationMessageService } from './services/operation-message.service';
import { TokenStoreService } from './services/token-store.service';
import { UserProfileProviderService } from './services/user-profile-provider.service';
import { AppMessageOpsatService } from './services/app-message-opsat.service';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ProfileComponent } from './components/profile/profile.component';
import { MainComponent } from './components/main/main.component';
import { ProfileResolver } from './resolvers/profile.resolver';
import { SocketClientService } from './services/socket-client.service';

export function apiGatewayFn(configSrv: EnvStoreService): string {
    const apiGateway: string = `${configSrv.getEnvConfig().apiGateway}`;
    return apiGateway;
}

export function appInitializerFn(store: fromCore.IEnvStore): Function {
    const fn: Function = () => store.loadEnvConfig();
    return fn;
}

@NgModule({
    declarations: [
        LoginComponent,
        NotFoundComponent,
        ProfileComponent,
        MainComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        LayoutModule,
        RouterModule,
        NzMessageModule,
        NzButtonModule,
        StoreModule.forRoot({}),
        EffectsModule.forRoot(),
        StoreDevtoolsModule.instrument(),
    ],
    providers: [
        AuthenticationGuard,
        AuthorizationGuard,
        EnvStoreService,
        ProfileResolver,
        { provide: NZ_I18N, useValue: zh_CN },
        { provide: LOCALE_ID, useValue: 'zh' },
        { provide: fromCore.ENV_STORE, useExisting: EnvStoreService },
        { provide: fromCore.IDENTITY_STORE, useClass: IdentityService },
        { provide: fromCore.TOKEN_STORE, useClass: TokenStoreService },
        { provide: fromCore.USER_PROFILE_PROVIDER, useClass: UserProfileProviderService },
        // { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
        // { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
        // { provide: HTTP_INTERCEPTORS, useClass: SocketInterceptor, multi: true },
        { provide: APP_INITIALIZER, useFactory: appInitializerFn, multi: true, deps: [fromCore.ENV_STORE] },
        { provide: fromCore.API_GATEWAY, useFactory: apiGatewayFn, deps: [EnvStoreService] },
        { provide: fromCore.OPERATION_MESSAGE, useClass: OperationMessageService },
        { provide: fromCore.APP_MESSAGE_OPSAT, useClass: AppMessageOpsatService },
        { provide: fromCore.SOCKET_CLIENT, useClass: SocketClientService },
    ],
    exports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        RouterModule,
        NzMessageModule,
        NzButtonModule,
    ]
})
export class StarterModule {
    public constructor(
        @Optional()
        @SkipSelf()
        parentModule: StarterModule
    ) {
        if (parentModule) {
            throw new Error('app模块使用,其他子模块不需要再引用了!');
        }
    }

    public static forRoot(): ModuleWithProviders<StarterModule> {
        return {
            ngModule: StarterModule,
            // providers: [...alainProvides, ...zorroProvides]
        };
    }
}
