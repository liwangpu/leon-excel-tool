import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ChangeDetectionStrategy, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import * as fromCore from 'workstation-core';
import { saveAs } from 'file-saver';

@Component({
    selector: 'app-compensation',
    templateUrl: './compensation.component.html',
    styleUrls: ['./compensation.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class CompensationComponent implements OnInit {

    public readonly form: FormGroup;
    public constructor(
        @Inject(fromCore.API_GATEWAY)
        private apiGateway: string,
        private fb: FormBuilder,
        private http: HttpClient,
    ) {
        this.form = this.fb.group({
            compensations: [],
            refunds: [],
            solution: [],
            departmentMap: []
        });
    }

    public ngOnInit(): void {
        // this.form.valueChanges.subscribe(val => {
        //     // console.log('ccc:', val);

        //     const formData = fromCore.transferToFormData(this.form, ['compensations', 'refunds']);
        //     console.log('compensations:', formData.getAll('compensations'));
        //     console.log('refunds:', formData.getAll('refunds'));
        // });
    }

    public async upload(): Promise<void> {
        const formData = fromCore.transferToFormData(this.form, ['compensations', 'refunds', 'solution', 'departmentMap']);
        this.http.post(`${this.apiGateway}/api/Compensation/Upload`, formData, { responseType: 'arraybuffer' })
            .subscribe(buffer => {
                saveAs(new Blob([buffer]), '处理结果.zip');
            });
    }

}
