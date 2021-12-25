import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NZ_I18N } from 'ng-zorro-antd/i18n';
import { zh_CN } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import zh from '@angular/common/locales/zh';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { HomeComponent } from './components/home/home.component';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { IconDefinition } from '@ant-design/icons-angular';
import { MenuFoldOutline, MenuUnfoldOutline } from '@ant-design/icons-angular/icons';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { AppConfigService } from './services';
import { API_GATEWAY } from './tokens';

registerLocaleData(zh);

const icons: Array<IconDefinition> = [MenuFoldOutline, MenuUnfoldOutline];

const appInitializerFn: Function = (appConfig: AppConfigService) =>
    () => appConfig.loadAppConfig();
const apiGatewayFn: Function = (configSrv: AppConfigService) => `${configSrv.appConfig?.apiGateway}`;

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        AppRoutingModule,
        FormsModule,
        HttpClientModule,
        NzMenuModule,
        NzButtonModule,
        NzIconModule.forRoot(icons),
    ],
    providers: [
        AppConfigService,
        { provide: NZ_I18N, useValue: zh_CN },
        {
            provide: APP_INITIALIZER,
            useFactory: appInitializerFn,
            multi: true,
            deps: [AppConfigService]
        },
        {
            provide: API_GATEWAY,
            useFactory: apiGatewayFn,
            deps: [AppConfigService]
        },
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
