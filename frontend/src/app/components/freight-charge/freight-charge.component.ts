import { HttpClient } from '@angular/common/http';
import { Component, ChangeDetectionStrategy, Inject, QueryList, ViewChildren } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as fromCore from 'workstation-core';
import { saveAs } from 'file-saver';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { UploaderComponent } from 'workstation-shared/attachment-uploader';

@Component({
  selector: 'app-freight-charge',
  templateUrl: './freight-charge.component.html',
  styleUrls: ['./freight-charge.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FreightChargeComponent {

  @ViewChildren(UploaderComponent)
  public readonly attachUploaders: QueryList<UploaderComponent>;
  public readonly form: FormGroup;
  public constructor(
    @Inject(fromCore.API_GATEWAY)
    private apiGateway: string,
    private fb: FormBuilder,
    private http: HttpClient,
    private notification: NzNotificationService
  ) {
    this.form = this.fb.group({
      freightChargeFile: [null, [Validators.required]]
    });
  }

  public async upload(): Promise<void> {
    const formData = fromCore.transferToFormData(this.form, ['freightChargeFile']);
    this.notification.info('温馨提示', '数据已经上传，结果待会会自己下载，请耐心等候');
    this.http.post(`${this.apiGateway}/api/FreightCharge/FreightChargeAnylysis`, formData, { responseType: 'arraybuffer' })
      .subscribe(buffer => {
        saveAs(new Blob([buffer]), '空海运差异比例处理结果.xlsx');
        this.attachUploaders.forEach(it => it.refreshAttachInfo());
      });
  }


}
