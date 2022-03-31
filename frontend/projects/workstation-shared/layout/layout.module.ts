import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { IconDefinition } from '@ant-design/icons-angular';
import * as fromIcon from '@ant-design/icons-angular/icons';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { LayoutComponent } from './components/layout/layout.component';
import { HeaderComponent } from './components/header/header.component';
import { NzButtonModule } from 'ng-zorro-antd/button';

const icons: Array<IconDefinition> = [fromIcon.MenuOutline, fromIcon.AppleOutline];

@NgModule({
    declarations: [
        LayoutComponent,
        HeaderComponent
    ],
    imports: [
        CommonModule,
        NzIconModule.forChild(icons),
        NzButtonModule,
        NzMenuModule,
        NzToolTipModule,
        NzDropDownModule,
        RouterModule
    ],
    providers: [
    ],
    exports: [
        LayoutComponent
    ]
})
export class LayoutModule { }
