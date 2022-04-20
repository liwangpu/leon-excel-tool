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
    public menuSwitchIcon: 'mo_retract_add' | 'mo_retract_no' = 'mo_retract_no';

    @fromStationCore.LazyService(StoreService)
    private readonly store: StoreService;
    public constructor(
        protected injector: Injector
    ) { }

    public ngOnInit(): void {
        //
    }

    public toggleMenuCollapsed(): void {
        this.menuSwitchIcon = this.menuSwitchIcon === 'mo_retract_add' ? 'mo_retract_no' : 'mo_retract_add';
        // tslint:disable-next-line: no-floating-promises
        this.store.toggleMenuCollapsed();
    }

}
