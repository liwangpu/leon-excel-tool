import { HttpClient } from '@angular/common/http';
import { Component, ChangeDetectionStrategy, Inject, QueryList, ViewChildren } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
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
            bill: []
        });
    }

    public async upload(): Promise<void> {
        const formData = fromCore.transferToFormData(this.form, ['bill']);
        this.notification.info('温馨提示', '数据已经上传，结果待会会自己下载，请耐心等候');
        this.http.post(`${this.apiGateway}/api/Compensation/Upload`, formData, { responseType: 'arraybuffer' })
            .subscribe(buffer => {
                saveAs(new Blob([buffer]), '空海运账单-处理结果.xlsx');
                this.attachUploaders.forEach(it => it.refreshAttachInfo());
            });
    }


}
