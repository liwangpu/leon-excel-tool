import { HttpClient } from '@angular/common/http';
import { Component, ChangeDetectionStrategy, Inject, ViewChildren, QueryList } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as fromCore from 'workstation-core';
import { saveAs } from 'file-saver';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { UploaderComponent } from 'workstation-shared/attachment-uploader';

@Component({
    selector: 'app-amazon-compensation',
    templateUrl: './amazon-compensation.component.html',
    styleUrls: ['./amazon-compensation.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class AmazonCompensationComponent {

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
            compensations: [null, [Validators.required]],
            storeChangeName: []
        });
    }

    public upload(): void {
        const formData: FormData = fromCore.transferToFormData(this.form, ['compensations', 'storeChangeName']);
        this.notification.info('温馨提示', '数据已经上传，结果待会会自己下载，请耐心等候');
        this.http.post(`${this.apiGateway}/api/Compensation/AmazonUpload`, formData, { responseType: 'arraybuffer' })
            .subscribe(buffer => {
                saveAs(new Blob([buffer]), '亚马逊索赔.xlsx');
                this.attachUploaders.forEach(it => it.refreshAttachInfo());
            });
    }

}
