import { Component, OnInit, ChangeDetectionStrategy, forwardRef, Input, ViewChild, HostListener, ElementRef, Injector, ChangeDetectorRef, HostBinding } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import * as fromCore from 'workstation-core';

const placeHolder: string = '点击选取文件';
@Component({
    selector: 'workstation-shared-uploader',
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
    public readonly accept: string;
    @Input()
    public readonly multiple: boolean;
    public message: string = placeHolder;
    public fileNames: Array<string>;
    public files: Array<File>;
    @HostBinding('attr.title')
    public title: string;
    @ViewChild('fileCtl', { read: ElementRef })
    public readonly fileCtl: ElementRef;
    @fromCore.LazyService(ChangeDetectorRef)
    private readonly cdr: ChangeDetectorRef;
    private onChangeFn: (val: any) => any;
    private onTouchedFn: () => any;
    public constructor(
        protected injector: Injector
    ) { }

    public ngOnInit(): void {
    }

    public writeValue(obj: any): void {
        //
    }

    @HostListener('click', ['$event'])
    public onClick(e: Event): void {
        this.fileCtl.nativeElement.click();
    }

    public readFiles(files: FileList): void {
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

}
