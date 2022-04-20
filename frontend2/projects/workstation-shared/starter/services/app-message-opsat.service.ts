import { Injectable, OnDestroy } from '@angular/core';
import { IMessageOpsat } from 'workstation-core';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class AppMessageOpsatService implements IMessageOpsat, OnDestroy {

    private _message$: Subject<{ topic: string; data: any }> = new Subject<{ topic: string; data: any }>();

    public get message$(): Observable<{ topic: string; data: any }> {
        return this._message$.asObservable();
    }

    public ngOnDestroy(): void {
        this._message$.complete();
        this._message$.unsubscribe();
    }

    public publish(topic: string, data?: any): void {
        this._message$.next({ topic, data });
    }
}
