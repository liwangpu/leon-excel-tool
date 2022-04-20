import { Component, OnInit, ChangeDetectionStrategy, Injector } from '@angular/core';
import { io, Socket } from 'socket.io-client';
import * as fromCore from 'workstation-core';

@Component({
    selector: 'workstation-shared-main',
    templateUrl: './main.component.html',
    styleUrls: ['./main.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MainComponent implements OnInit {


    @fromCore.LazyService(fromCore.SOCKET_CLIENT)
    private readonly socketClient: fromCore.ISocketClient;
    public constructor(
        protected injector: Injector
    ) { }

    public ngOnInit(): void {
        this.socketClient.connect();
    }
}
