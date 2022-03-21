import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { IconDefinition } from '@ant-design/icons-angular';
import * as fromIcon from '@ant-design/icons-angular/icons';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { BannerComponent } from './components/banner/banner.component';
import { LayoutComponent } from './components/layout/layout.component';
import { ProfileBarComponent } from './components/profile-bar/profile-bar.component';
import { ProfilePanelComponent } from './components/profile-panel/profile-panel.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { StoreService } from './services/store.service';

const icons: Array<IconDefinition> = [fromIcon.MenuOutline, fromIcon.AppleOutline];

@NgModule({
    declarations: [
        LayoutComponent,
        BannerComponent,
        SidebarComponent,
        ProfileBarComponent,
        ProfilePanelComponent
    ],
    imports: [
        CommonModule,
        NzIconModule.forChild(icons),
        NzMenuModule,
        NzToolTipModule,
        NzDropDownModule,
        RouterModule
    ],
    providers: [
        StoreService
    ],
    exports: [
        LayoutComponent
    ]
})
export class LayoutModule { }
