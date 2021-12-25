import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { API_GATEWAY } from 'src/app/tokens';

interface IFormValue {
  datas: Array<any>;
}

@Component({
  selector: 'app-other-edit',
  templateUrl: './other-edit.component.html',
  styleUrls: ['./other-edit.component.scss']
})
export class OtherEditComponent {

  public uploadUrl: string;
  public form: FormGroup;
  public constructor(
    @Inject(API_GATEWAY)
    protected apiGateway: string,
    @Inject(NzModalRef)
    protected modal: NzModalRef,
    protected fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      datas: []
    });
    this.uploadUrl = `${this.apiGateway}/EmployedStatistic/UploadOther`;
  }

  public cancel(): void {
    this.modal.close();
  }

  public async save(): Promise<void> {
    let data: IFormValue = this.form.value;
    // console.log('form:', data);
    this.modal.close(data);
  }

}
