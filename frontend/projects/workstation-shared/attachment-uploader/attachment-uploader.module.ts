import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UploaderComponent } from './uploader/uploader.component';
import { IconDefinition } from '@ant-design/icons-angular';
import * as fromIcon from '@ant-design/icons-angular/icons';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';

const icons: Array<IconDefinition> = [fromIcon.DownloadOutline];

@NgModule({
    declarations: [
        UploaderComponent
    ],
    imports: [
        CommonModule,
        NzButtonModule,
        NzIconModule.forChild(icons)
    ],
    exports: [
        UploaderComponent
    ]
})
export class AttachmentUploaderModule { }
