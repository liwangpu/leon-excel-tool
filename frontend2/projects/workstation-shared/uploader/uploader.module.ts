import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UploaderComponent } from './uploader.component';


@NgModule({
    declarations: [
        UploaderComponent
    ],
    imports: [
        CommonModule
    ],
    exports: [
        UploaderComponent
    ]
})
export class UploaderModule { }
