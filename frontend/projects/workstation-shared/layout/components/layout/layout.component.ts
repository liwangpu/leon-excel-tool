import { Component, Injector, OnInit } from '@angular/core';
import * as fromStationCore from 'workstation-core';
import * as fromModel from '../../models';
import { StoreService } from '../../services/store.service';
import { ILayoutStartup, LAYOUT_STARTUP } from '../../tokens/layout-startup';

@Component({
    selector: 'workstation-shared-layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {

    @fromStationCore.LazyService(StoreService)
    private readonly store: StoreService;
    @fromStationCore.LazyService(LAYOUT_STARTUP)
    private readonly layoutStartup: ILayoutStartup;
    public constructor(
        protected injector: Injector
    ) { }

    public async ngOnInit(): Promise<void> {
        const appName: string = await this.layoutStartup.getAppName();
        await this.store.updateAppName(appName);
        const menus: Array<fromModel.IMenu> = await this.layoutStartup.getMenus();
        await this.store.updateMenus(menus);
    }

}
