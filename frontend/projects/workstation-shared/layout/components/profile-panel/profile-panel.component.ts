import { APP_BASE_HREF } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import * as fromStationCore from 'workstation-core';
import { SubSink } from 'subsink';
import { ITenant } from '../../models';
import { StoreService } from '../../services/store.service';
import { ILayoutStartup, LAYOUT_STARTUP } from '../../tokens/layout-startup';

@Component({
    selector: 'workstation-shared-layout-profile-panel',
    templateUrl: './profile-panel.component.html',
    styleUrls: ['./profile-panel.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class ProfilePanelComponent implements OnInit, OnDestroy, OnChanges {
    public changeTenanting: boolean = false;
    @Input()
    public appVersion: string;
    @Input()
    public userInfo: any;
    @Input()
    public avatarColor: string;
    @Input()
    public departmentName: string;
    public userAvatar: string;
    public avatarSwitchName: string;
    public activeTenant: ITenant;
    public isShow: boolean = true;
    public tenants: Array<ITenant>;
    @Output()
    private readonly afterActionTrigger: EventEmitter<void> = new EventEmitter<void>();
    @fromStationCore.LazyService(fromStationCore.IDENTITY_STORE)
    private readonly identityStore: fromStationCore.IdentityStore;
    @fromStationCore.LazyService(fromStationCore.TOKEN_STORE)
    private readonly tokenStore: fromStationCore.ITokenStore;
    @fromStationCore.LazyService(StoreService)
    private readonly store: StoreService;
    @fromStationCore.LazyService(Router)
    private readonly router: Router;
    @fromStationCore.LazyService(LAYOUT_STARTUP)
    private readonly layoutStartup: ILayoutStartup;
    @fromStationCore.LazyService(ChangeDetectorRef)
    private readonly cdr: ChangeDetectorRef;
    private readonly subs: SubSink = new SubSink();

    public constructor(protected injector: Injector) {
    }

    public ngOnChanges(changes: SimpleChanges): void {
        this.userAvatar = this.userInfo?.avatar;
        this.avatarSwitchName = this.userInfo?.name[this.userInfo.name.length - 1];
        this.getActiveTenant(this.userInfo);
    }

    public ngOnDestroy(): void {
        this.subs.unsubscribe();
    }

    public async ngOnInit(): Promise<void> {
        this.subs.sink = this.identityStore.queryTenantList({})
            .subscribe(tenants => {
                this.tenants = tenants;
                this.getActiveTenant(this.userInfo);
                this.cdr.markForCheck();
            });
    }

    public toSwitch(): void {
        this.isShow = !this.isShow;
    }

    public async logout(): Promise<void> {
        this.tokenStore.clearToken();
        this.reloadApp();
    }

    public reset(): void {
        this.isShow = true;
        this.cdr.markForCheck();
    }

    public leaveTenant(): void {
        this.isShow = true;
    }

    public async changeTenant(id: string): Promise<void> {
        if (this.changeTenanting) {
            return;
        }
        this.changeTenanting = true;

        const tokenInfo: fromStationCore.ITokenInfo = await this.identityStore.refreshToken(id).toPromise();
        await this.tokenStore.setToken(tokenInfo);
        this.changeTenanting = false;
        this.reloadApp();
    }


    public async profileSetting(): Promise<void> {
        // if (!this.layoutStartup.userProfileRoute) {
        //     await this.router.navigateByUrl(`${this.appBaseHref}/profile`);
        // } else {
        //     await this.router.navigateByUrl(this.layoutStartup.userProfileRoute());
        // }

        // this.afterActionTrigger.emit();
    }

    private getActiveTenant(userInfo: any): void {
        if (!userInfo || !this.tenants) {
            return;
        }
        this.activeTenant = this.tenants.find(t => t.id === this.userInfo?.tenantId);
        this.cdr.markForCheck();
    }

    private reloadApp(): void {
        // 因为token更新需要向其他独立应用发送广播,不要马上reload
        setTimeout(() => {
            location.reload();
        }, 250);
    }
}
