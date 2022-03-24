import { ModuleWithProviders } from '@angular/core';
import { Route, Routes } from '@angular/router';
import { IconDefinition } from '@ant-design/icons-angular';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { LoginComponent } from './components/login/login.component';
import { MainComponent } from './components/main/main.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ProfileComponent } from './components/profile/profile.component';
import { AuthenticationGuard } from './guards/authentication.guard';
import { ProfileResolver } from './resolvers/profile.resolver';

const icons: Array<IconDefinition> = [];

export const APP_USERPROFILE_ROUTE: Route = { path: 'profile', component: ProfileComponent };

export const APP_LAYOUT_ROUTE: Route = {
    path: '',
    component: MainComponent,
    canActivate: [AuthenticationGuard],
    resolve: {
        profile: ProfileResolver
    }
};

export const APP_COMMON_ROUTERS: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'not-found', component: NotFoundComponent },
    { path: '**', redirectTo: 'not-found' }
];

export function standardSubAppRouterGenerator(subAppRoutes: Routes = [], additionalTopLevelRoutes: Routes = []): Routes {
    // TestComponent临时测试用,暂时留一下
    return [
        {
            ...APP_LAYOUT_ROUTE,
            children: [
                ...subAppRoutes,
                APP_USERPROFILE_ROUTE,
                // { path: 'build-in-test', component: TestComponent },
            ]
        },
        ...additionalTopLevelRoutes,
        ...APP_COMMON_ROUTERS
    ];
}

export function AppIconSetting(additionalIcons: Array<IconDefinition> = []): ModuleWithProviders<NzIconModule> {
    return NzIconModule.forRoot([
        ...icons,
        ...additionalIcons
    ]);
}
