import { HttpClient, HttpEvent, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { Component, OnInit, ChangeDetectionStrategy, Input, Injector, ChangeDetectorRef, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { NzUploadFile, NzUploadXHRArgs } from 'ng-zorro-antd/upload';
import { Subscription } from 'rxjs';
import * as fromService from '../../services';
import { API_GATEWAY } from 'src/app/tokens';
import { OtherEditComponent } from '../other-edit/other-edit.component';
import { filter } from 'rxjs/operators';

interface IFormValue {

}

@Component({
  selector: 'app-other',
  templateUrl: './other.component.html',
  styleUrls: ['./other.component.scss']
})
export class OtherComponent implements OnInit {

  public datas: Array<Object> = [];

  public constructor(
    @Inject(NzModalService)
    private readonly modal: NzModalService
  ) {
  }

  public ngOnInit(): void {
  }

  public export(): void {

  }

  public uploadExcel(): void {
    const ref = this.modal.create({
      nzTitle: '上传表格',
      nzContent: OtherEditComponent,
      // nzViewContainerRef: this.viewContainerRef,
      nzWrapClassName: 'excel-tool-modal',
      nzWidth: 700,
      nzMaskClosable: false,
      nzFooter: null,
      nzCentered: true,
      nzCancelText: null,
      nzOkText: null,
      nzComponentParams: {
        // initValue: {
        //   componentId: this.componentId
        // }
      },
    });

    ref.afterClose
      .pipe(filter(res => res ? true : false))
      .subscribe((res) => {
        console.log('res:', res);
        this.datas = res.datas;

        // this.gridRefreshFn();
        // this.refreshComponent.emit();
      });
  }



}
