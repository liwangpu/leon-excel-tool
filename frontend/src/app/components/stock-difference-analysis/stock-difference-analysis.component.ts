import { HttpClient } from '@angular/common/http';
import { Component, ChangeDetectionStrategy, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as fromCore from 'workstation-core';
import { saveAs } from 'file-saver';
import * as moment from 'moment';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
    selector: 'app-stock-difference-analysis',
    templateUrl: './stock-difference-analysis.component.html',
    styleUrls: ['./stock-difference-analysis.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class StockDifferenceAnalysisComponent {

    public readonly form: FormGroup;
    public constructor(
        @Inject(fromCore.API_GATEWAY)
        private apiGateway: string,
        private fb: FormBuilder,
        private http: HttpClient,
        private notification: NzNotificationService
    ) {
        this.form = this.fb.group({
            detailFile: [null, [Validators.required]],
            statisticalTime: [new Date(), [Validators.required]]
        });
    }

    public async upload(): Promise<void> {
        const formData = fromCore.transferToFormData(this.form, ['detailFile']);
        const statisticalTime = moment(this.form.value.statisticalTime).format('YYYY-MM-DD');
        formData.set('statisticalTime', statisticalTime);
        this.notification.info('温馨提示', '数据已经上传，结果待会会自己下载，请耐心等候');
        this.http.post(`${this.apiGateway}/api/StockAnalysis/DifferenceAnylysis`, formData, { responseType: 'arraybuffer' })
            .subscribe(buffer => {
                saveAs(new Blob([buffer]), '库存差异分析-处理结果.xlsx');
            });
    }

}
