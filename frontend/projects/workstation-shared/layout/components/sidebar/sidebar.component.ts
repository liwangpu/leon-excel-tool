import { ChangeDetectionStrategy, ChangeDetectorRef, Component, HostBinding, Injector, OnDestroy, OnInit } from '@angular/core';
import * as fromStationCore from 'workstation-core';
import { NzMenuItemDirective } from 'ng-zorro-antd/menu';
import { combineLatest } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { SubSink } from 'subsink';
import * as fromModel from '../../models';
import { StoreService } from '../../services/store.service';

@Component({
    selector: 'workstation-shared-layout-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent implements OnInit, OnDestroy {

    public appName: string;
    public menus: Array<fromModel.IMenu>;
    @HostBinding('class.collapsed')
    public menuCollapsed: boolean;
    @fromStationCore.LazyService(ChangeDetectorRef)
    public readonly cdr: ChangeDetectorRef;
    @fromStationCore.LazyService(StoreService)
    public readonly store: StoreService;
    private subs: SubSink = new SubSink();
    public constructor(
        protected injector: Injector
    ) { }

    public ngOnDestroy(): void {
        this.subs.unsubscribe();
    }

    public ngOnInit(): void {
        this.subs.sink = combineLatest([
            this.store.appName$,
            this.store.menus$,
            this.store.menuCollapsed$
        ])
            .pipe(debounceTime(100))
            .subscribe(([appName, menus, collapsed]) => {
                this.appName = appName;
                this.menus = menus;
                this.menuCollapsed = collapsed;
                this.cdr.markForCheck();
            });
    }

    public async menuClick(menu: NzMenuItemDirective): Promise<void> {
        // console.log('menu:', menu);
    }

}
