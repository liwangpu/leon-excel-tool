import { Injectable } from '@angular/core';
import { IOperationMessage } from '@cxist/mirror-workstation-core';
import { NzMessageService } from 'ng-zorro-antd/message';

@Injectable()
export class OperationMessageService implements IOperationMessage {

    public constructor(private messageSrv: NzMessageService) {
    }

    public success(message: string): void {
        this.messageSrv.success(message);
    }

    public info(message: string): void {
        this.messageSrv.info(message);
    }

    public warn(message: string): void {
        this.messageSrv.warning(message);
    }

    public error(message: string): void {
        this.messageSrv.error(message);
    }
}
