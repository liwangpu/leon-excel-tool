import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { standardSubAppRouterGenerator } from 'workstation-shared/starter';
import { HomeComponent } from './components/home/home.component';

const routes: Routes = standardSubAppRouterGenerator([
    { path: 'home', component: HomeComponent },
    // { path: 'management', loadChildren: () => import('./app-loaders/platform-manangement-loader.module').then(m => m.PlatformManangementLoaderModule) },
    { path: '', pathMatch: 'full', redirectTo: 'home' }
]);

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
