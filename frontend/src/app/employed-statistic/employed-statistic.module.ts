import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzInputModule } from 'ng-zorro-antd/input';
import { EmployedStatisticRoutingModule } from './employed-statistic-routing.module';
import { OtherComponent } from './components/other/other.component';
import { EmployedStatisticService } from './services';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ExcelUploaderComponent } from './components/excel-uploader/excel-uploader.component';
import { OtherEditComponent } from './components/other-edit/other-edit.component';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { EditModalComponent } from './components/edit-modal/edit-modal.component';

@NgModule({
  declarations: [
    OtherComponent,
    ExcelUploaderComponent,
    OtherEditComponent,
    EditModalComponent
  ],
  imports: [
    CommonModule,
    EmployedStatisticRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NzInputModule,
    NzInputNumberModule,
    NzUploadModule,
    NzButtonModule,
    NzModalModule,
  ],
  providers: [
    EmployedStatisticService
  ]
})
export class EmployedStatisticModule { }
