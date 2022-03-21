import { ChangeDetectionStrategy, Component, Injector, OnInit } from '@angular/core';
import * as fromStationCore from 'workstation-core';
import { StoreService } from '../../services/store.service';

@Component({
    selector: 'workstation-shared-layout-banner',
    templateUrl: './banner.component.html',
    styleUrls: ['./banner.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class BannerComponent implements OnInit {

    @fromStationCore.LazyService(StoreService)
    private readonly store: StoreService;
    public constructor(
        protected injector: Injector
    ) { }

    public ngOnInit(): void {
        //
    }

    public toggleMenuCollapsed(): void {
        // tslint:disable-next-line: no-floating-promises
        this.store.toggleMenuCollapsed();
    }

}
