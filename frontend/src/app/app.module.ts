import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { StarterModule } from "workstation-shared/starter";

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent
    ],
    imports: [
        StarterModule,
        AppRoutingModule
    ],
    providers: [
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
