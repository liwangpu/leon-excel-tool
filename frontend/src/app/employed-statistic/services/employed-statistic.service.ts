import { HttpRequest } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_GATEWAY } from 'src/app/tokens';

@Injectable()
export class EmployedStatisticService {

  public url: string;
  public constructor(
    @Inject(API_GATEWAY)
    protected apiGateway: string
  ) {
    this.url = `${this.apiGateway}/api/EmployedStatistic`
  }

  // public uploadOther(formData: FormData): Observable<any> {
  //   const req = new HttpRequest('POST', `${this.url}/UploadOther`, formData, {
  //     reportProgress: true,
  //     withCredentials: false
  //   });

  //   return this.http
  //     .request(req).subscribe((event: HttpEvent<{}>) => {
  //       if (event.type === HttpEventType.UploadProgress) {
  //         if (event.total > 0) {
  //           (event as any).percent = event.loaded / event.total * 100; // tslint:disable-next-line:no-any
  //         }
  //         // To process the upload progress bar, you must specify the `percent` attribute to indicate progress.
  //         item.onProgress(event, item.file);
  //       } else if (event instanceof HttpResponse) { /* success */
  //         item.onSuccess(event.body, item.file, event);
  //         this.form.patchValue({ uploaded: true });
  //         this.cd.markForCheck();
  //       }
  //     }, err => {
  //       item.onError(err, item.file);
  //       this.cd.markForCheck();
  //     });
  // }
}
