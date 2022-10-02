import { HttpClient } from '@angular/common/http';
import { Component, ChangeDetectionStrategy, Inject, ViewChildren, QueryList } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as fromCore from 'workstation-core';
import { saveAs } from 'file-saver';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { UploaderComponent } from 'workstation-shared/attachment-uploader';

@Component({
  selector: 'app-alipay-intel-bill-collect',
  templateUrl: './alipay-intel-bill-collect.component.html',
  styleUrls: ['./alipay-intel-bill-collect.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AlipayIntelBillCollectComponent {

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
      packages: [null, [Validators.required]]
    });
  }

  public upload(): void {
    const formData: FormData = fromCore.transferToFormData(this.form, ['packages']);
    this.notification.info('温馨提示', '数据已经上传，结果待会会自己下载，请耐心等候');
    this.http.post(`${this.apiGateway}/api/AliExpress/AlipayIntlBillCollectUpload`, formData, { responseType: 'arraybuffer' })
      .subscribe(buffer => {
        saveAs(new Blob([buffer]), '速卖通支付宝国际账单汇总.xlsx');
        this.attachUploaders.forEach(it => it.refreshAttachInfo());
      });
  }
}
