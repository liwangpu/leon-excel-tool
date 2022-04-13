import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { standardSubAppRouterGenerator } from 'workstation-shared/starter';
import { CompensationComponent } from './components/compensation/compensation.component';
import { HomeComponent } from './components/home/home.component';
import { TestComponent } from './components/test/test.component';

const routes: Routes = standardSubAppRouterGenerator([
    { path: 'home', component: HomeComponent },
    { path: 'test', component: TestComponent },
    { path: 'compensation', component: CompensationComponent },
    // { path: 'management', loadChildren: () => import('./app-loaders/platform-manangement-loader.module').then(m => m.PlatformManangementLoaderModule) },
    { path: '', pathMatch: 'full', redirectTo: 'home' }
]);

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
