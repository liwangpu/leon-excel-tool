import { HttpClient } from '@angular/common/http';
import { Component, OnInit, Inject } from '@angular/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { API_GATEWAY } from 'src/app/tokens';
import { OtherEditComponent } from '../other-edit/other-edit.component';
import { filter } from 'rxjs/operators';
import { saveAs } from 'file-saver';
import * as faker from 'faker';

@Component({
  selector: 'app-other',
  templateUrl: './other.component.html',
  styleUrls: ['./other.component.scss']
})
export class OtherComponent implements OnInit {

  public datas: Array<Object> = [];
  public exportUrl: string;
  public constructor(
    @Inject(NzModalService)
    private readonly modal: NzModalService,
    @Inject(API_GATEWAY)
    protected apiGateway: string,
    protected readonly http: HttpClient,
  ) {
    this.exportUrl = `${this.apiGateway}/EmployedStatistic/ExportOther`;
  }

  public ngOnInit(): void {

  }

  public exportExcel(): void {
    this.http.post(this.exportUrl, { datas: this.datas }, { responseType: 'blob' })
      .subscribe(res => {
        saveAs(res, `${faker.datatype.uuid()}.xlsx`);
      });
  }

  public uploadExcel(): void {
    const ref = this.modal.create({
      nzTitle: '上传表格',
      nzContent: OtherEditComponent,
      nzWrapClassName: 'excel-tool-modal',
      nzWidth: 700,
      nzMaskClosable: false,
      nzFooter: null,
      nzCentered: true,
      nzCancelText: null,
      nzOkText: null,
      nzComponentParams: {
      },
    });

    ref.afterClose
      .pipe(filter(res => res ? true : false))
      .subscribe((res) => {
        this.datas = res.datas;
      });
  }

}
