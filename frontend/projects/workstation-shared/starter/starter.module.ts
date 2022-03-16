import { NgModule } from '@angular/core';
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
import { NzMessageModule } from 'ng-zorro-antd/message';

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

@NgModule({
  declarations: [
    LoginComponent
  ],
  imports: [
    CommonModule
  ]
})
export class StarterModule { }
