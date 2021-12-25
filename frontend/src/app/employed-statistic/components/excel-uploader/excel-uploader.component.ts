import { HttpClient, HttpEvent, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { NzUploadFile, NzUploadXHRArgs } from 'ng-zorro-antd/upload';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-excel-uploader',
  templateUrl: './excel-uploader.component.html',
  styleUrls: ['./excel-uploader.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ExcelUploaderComponent),
      multi: true
    }
  ]
})
export class ExcelUploaderComponent implements ControlValueAccessor {

  @Input()
  public uploadUrl: string;
  public fileList: NzUploadFile[] = [];
  public fileUploadRequest: (item: NzUploadXHRArgs) => Subscription = item => this.fileUpload(item);
  public beforeUpload = (file: NzUploadFile): boolean => {
    this.fileList = this.fileList.concat(file);
    return true;
  };
  private datas: Array<any> = [];
  private onChangeFn: (val: any) => void;
  private onTouchFn: () => void;
  public constructor(
    protected readonly http: HttpClient,
  ) { }

  public async writeValue(value?: string): Promise<void> {

  }

  public registerOnChange(fn: any): void {
    this.onChangeFn = fn;
  }

  public registerOnTouched(fn: any): void {
    this.onTouchFn = fn;
  }

  public fileUpload(item: NzUploadXHRArgs): Subscription {
    const formData = new FormData();
    this.fileList.forEach((file: any) => {
      formData.append('files', file);
    });


    const req = new HttpRequest('POST', this.uploadUrl, formData, {
      reportProgress: true,
      withCredentials: false
    });

    return this.http
      .request(req).subscribe((event: HttpEvent<{}>) => {
        if (event.type === HttpEventType.UploadProgress) {
          if (event.total > 0) {
            (event as any).percent = event.loaded / event.total * 100; // tslint:disable-next-line:no-any
          }
          // To process the upload progress bar, you must specify the `percent` attribute to indicate progress.
          item.onProgress(event, item.file);
        } else if (event instanceof HttpResponse) { /* success */
          item.onSuccess(event.body, item.file, event);
          const list: Array<any> = event.body as any;
          if (list?.length) {
            list.forEach(d => this.datas.push(d));
          }
          this.onChangeFn(this.datas);
        }
      }, err => {
        item.onError(err, item.file);
      });
  }

}
