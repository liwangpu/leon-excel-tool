import { APP_INITIALIZER, LOCALE_ID, NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule, registerLocaleData } from '@angular/common';
import { LoginComponent } from './components/login/login.component';
import * as fromCore from 'workstation-core';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NZ_CONFIG } from 'ng-zorro-antd/core/config';
import { NzConfig } from 'ng-zorro-antd/core/config';
import { NZ_I18N, zh_CN } from 'ng-zorro-antd/i18n';
import { NzIconService } from 'ng-zorro-antd/icon';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { EnvStoreService } from './services/env-store.service';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import zh from '@angular/common/locales/zh';
import { AuthenticationGuard } from './guards/authentication.guard';
import { AuthorizationGuard } from './guards/authorization.guard';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { OperationMessageService } from './services/operation-message.service';
import { AppMessageOpsatService } from './services/app-message-opsat.service';
import { IdentityService } from './services/identity.service';

const ngZorroConfig: NzConfig = {
    message: { nzTop: 42 }
};
export function apiGatewayFn(configSrv: EnvStoreService): string {
    // tslint:disable-next-line: prefer-immediate-return
    const apiGateway: string = `${configSrv.getEnvConfig().apiGateway}`;
    return apiGateway;
}

export function appInitializerFn(store: fromCore.IEnvStore): Function {
    // tslint:disable-next-line: prefer-immediate-return
    const fn: Function = () => store.loadEnvConfig();
    return fn;
}

registerLocaleData(zh);

@NgModule({
    declarations: [
        LoginComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        RouterModule,
        NzMessageModule,
        NzButtonModule,
        StoreModule.forRoot({}),
        EffectsModule.forRoot(),
        StoreDevtoolsModule.instrument(),
    ],
    providers: [
        EnvStoreService,
        { provide: NZ_I18N, useValue: zh_CN },
        { provide: NZ_CONFIG, useValue: ngZorroConfig },
        { provide: LOCALE_ID, useValue: 'zh' },
        { provide: fromCore.ENV_STORE, useExisting: EnvStoreService },
        { provide: fromCore.IDENTITY_STORE, useClass: IdentityService },
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
        { provide: APP_INITIALIZER, useFactory: appInitializerFn, multi: true, deps: [fromCore.ENV_STORE] },
        { provide: fromCore.API_GATEWAY, useFactory: apiGatewayFn, deps: [EnvStoreService] },
        { provide: fromCore.OPERATION_MESSAGE, useClass: OperationMessageService },
        { provide: fromCore.APP_MESSAGE_OPSAT, useClass: AppMessageOpsatService },
    ]
})
export class StarterModule {
    public constructor(
        @Optional() @SkipSelf()
        parentModule: StarterModule,
        iconService: NzIconService,
    ) {
        if (parentModule) {
            throw new Error('app模块使用,其他子模块不需要再引用了!');
        }
        iconService.fetchFromIconfont({
            scriptUrl: '//at.alicdn.com/t/font_2555158_243jkp353g2i.js'
        });
    }
}
