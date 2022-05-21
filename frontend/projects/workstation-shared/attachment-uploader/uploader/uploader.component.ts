import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ChangeDetectionStrategy, forwardRef, Input, ViewChild, HostListener, ElementRef, Injector, ChangeDetectorRef, HostBinding } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import * as fromCore from 'workstation-core';
import { saveAs } from 'file-saver';

const placeHolder: string = '点击选取文件';

@Component({
    selector: 'workstation-shared-attachment-uploader',
    templateUrl: './uploader.component.html',
    styleUrls: ['./uploader.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => UploaderComponent),
            multi: true
        }
    ]
})
export class UploaderComponent implements ControlValueAccessor, OnInit {

    @Input()
    public readonly fileKey: string;
    @Input()
    public readonly accept: string;
    public hasAttachment: boolean;
    public downloading: boolean = false;
    public message: string = placeHolder;
    public fileNames: Array<string>;
    public files: Array<File>;
    @HostBinding('attr.title')
    public title: string;
    @fromCore.LazyService(fromCore.API_GATEWAY)
    private apiGateway: string;
    @ViewChild('fileCtl', { read: ElementRef })
    public readonly fileCtl: ElementRef;
    @fromCore.LazyService(ChangeDetectorRef)
    private readonly cdr: ChangeDetectorRef;
    @fromCore.LazyService(HttpClient)
    private readonly http: HttpClient;
    private onChangeFn: (val: any) => any;
    private onTouchedFn: () => any;
    public constructor(
        protected injector: Injector
    ) { }

    public async ngOnInit(): Promise<void> {
        if (this.fileKey) {
            await this.refreshAttachInfo();
        }
    }

    public writeValue(obj: any): void {
        //
    }

    public selectFile(): void {
        this.fileCtl.nativeElement.click();
    }

    public downloadAttactment(): void {
        // console.log('download:',);
        this.downloading = true;
        this.cdr.markForCheck();
        const url = `${this.apiGateway}/api/file/download/${this.fileKey}`;
        this.http.get(url, { responseType: 'blob' }).subscribe(res => {
            saveAs(res, this.fileKey);
            this.downloading = false;
            this.cdr.markForCheck();
        });
    }

    public readFiles(event: any): void {
        const files: FileList = event.target.files;
        if (!files.length) return;
        this.fileNames = [];
        this.files = [];
        for (let i = 0; i < files.length; i++) {
            const f: File = files[i];
            this.fileNames.push(f.name);
            this.files.push(f);
        }
        this.message = this.fileNames.join(',');
        this.title = `选中${this.fileNames.length}个文件: ${this.message}`;
        if (this.onChangeFn) {
            this.onChangeFn(this.files);
        }
        this.cdr.markForCheck();
    }

    public registerOnChange(fn: any): void {
        this.onChangeFn = fn;
    }

    public registerOnTouched(fn: any): void {
        this.onTouchedFn = fn;
    }

    public setDisabledState?(isDisabled: boolean): void {
        // 
    }

    public async refreshAttachInfo(): Promise<void> {
        const url = `${this.apiGateway}/api/file/check/${this.fileKey}`;
        this.hasAttachment = await this.http.get<boolean>(url).toPromise();
        if (this.hasAttachment) {
            this.message = this.fileKey;
        } else {
            this.message = placeHolder;
        }
        this.cdr.markForCheck();
    }


}
