import { HttpClient, HttpEvent, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { LazyService, API_GATEWAY } from '@pucst/core';
@Component({
    selector: 'app-stock-analysis',
    templateUrl: './stock-analysis.component.html',
    styleUrls: ['./stock-analysis.component.scss']
})
export class StockAnalysisComponent implements OnInit {

    public readonly form: FormGroup;
    public readonly uploadUrl: string;
    @LazyService(API_GATEWAY)
    private readonly apiGateway: string;
    @LazyService(HttpClient)
    protected readonly http: HttpClient;
    @LazyService(FormBuilder)
    private readonly fb: FormBuilder;
    public constructor(
        protected injector: Injector
    ) {
        this.uploadUrl = `${this.apiGateway}/StockAnalysis/upload`;
        this.form = this.fb.group({
            name: ['测试'],
            files: []
        });
    }

    public ngOnInit(): void {
        console.log('title', this.uploadUrl);
    }

    public onChange(fs: FileList): void {
        // console.log('files:', files);
        const files = [];
        for (let idx = fs.length - 1; idx >= 0; idx--) {
            files.push(fs[idx]);
        }
        this.form.patchValue({ files });
        // console.log('title:', this.form.value);

    }

    public upload(): void {
        console.log('value:', this.form.value);
        const formData = new FormData();
        formData.append("name", this.form.value.name);
        // formData.append("files", this.form.value.files);
        const files: Array<File> = this.form.value.files;
        files.forEach(f => {
            formData.append("files", f);
        });
        // console.log('title:', this.form.value.files);
        // console.log('formData:', formData.getAll('files'));
        const req = new HttpRequest('POST', this.uploadUrl, formData, {
            reportProgress: true,
            withCredentials: false
        });

        this.http
            .request(req)
            .subscribe((event: HttpEvent<{}>) => {
                if (event.type === HttpEventType.UploadProgress) {
                    if (event.total > 0) {
                        (event as any).percent = event.loaded / event.total * 100; // tslint:disable-next-line:no-any
                    }
                    // To process the upload progress bar, you must specify the `percent` attribute to indicate progress.
                    // item.onProgress(event, item.file);
                    // console.log('percent:', (event as any).percent);
                } else if (event instanceof HttpResponse) { /* success */
                    // item.onSuccess(event.body, item.file, event);
                    const list: Array<any> = event.body as any;
                    // if (list?.length) {
                    //     list.forEach(d => this.datas.push(d));
                    // }

                    // if (this.onChangeFn) {
                    //     this.onChangeFn(this.datas);
                    // }
                    console.log('success:');
                }
            }, err => {
                // item.onError(err, item.file);
                console.error('err:', err);
            });
    }
}
