import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { IconDefinition } from '@ant-design/icons-angular';
import { HomeComponent } from './components/home/home.component';
import { StarterModule, AppIconSetting } from "workstation-shared/starter";

const icons: Array<IconDefinition> = [];

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent
    ],
    imports: [
        StarterModule,
        AppRoutingModule,
        AppIconSetting(icons)
    ],
    providers: [
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
