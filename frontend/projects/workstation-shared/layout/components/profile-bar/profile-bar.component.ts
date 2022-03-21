import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Injector, OnDestroy, OnInit, ViewChild } from '@angular/core';
import * as fromStationCore from 'workstation-core';
import * as _ from 'lodash';
import { SubSink } from 'subsink';
import { ProfilePanelComponent } from '../profile-panel/profile-panel.component';

@Component({
    selector: 'workstation-shared-layout-profile-bar',
    templateUrl: './profile-bar.component.html',
    styleUrls: ['./profile-bar.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileBarComponent implements OnInit, OnDestroy {

    @ViewChild('profilePanel', { static: false })
    public profilePanel: ProfilePanelComponent;
    public showMenu: boolean = false;
    public userInfo: any;
    public userAvatar: string;
    public backgroundColor: string;
    public avatarSwitchName: string;
    public departmentName: string;
    @fromStationCore.LazyService(ChangeDetectorRef)
    public readonly cdr: ChangeDetectorRef;
    @fromStationCore.LazyService(fromStationCore.USER_PROFILE_PROVIDER)
    private readonly userProfileProvider: fromStationCore.IUserProfileProvider;
    // @fromStationCore.LazyService(fromStationCore.UPLOAD_HOST)
    // private readonly uploadHost: string;
    // @fromStationCore.LazyService(fromStationCore.DEPARTMENT_STORE)
    // private readonly departmentStore: fromStationCore.IDepartmentStore;
    private readonly subs: SubSink = new SubSink();
    public constructor(
        protected injector: Injector
    ) {
    }

    public ngOnDestroy(): void {
        this.subs.unsubscribe();
    }

    public async ngOnInit(): Promise<void> {
        // const currentUser: fromStationCore.IUserProfile = await this.userProfileProvider.getProfile();
        // this.userInfo = _.cloneDeep(currentUser);
        // const { avatar } = this.userInfo;
        // this.userInfo.avatar = avatar ? fromStationCore.UrlTool.replaceFileHost(avatar, this.uploadHost) : null;
        // // this.departmentName = '测试';
        // this.userAvatar = this.userInfo.avatar;
        // this.avatarSwitchName = this.userInfo?.name[this.userInfo?.name.length - 1];
        // if (!this.userAvatar) {
        //     this.avatarSwitchName = this.userInfo?.name[this.userInfo?.name.length - 1];
        //     // this.backgroundColor = fromStationCore.calculateAvatarColor(this.userInfo.employeeId);
        //     this.backgroundColor = 'orange';
        // }
        // const deps = await this.departmentStore.queryByIds(currentUser.departmentIds).toPromise();
        // this.cdr.markForCheck();
    }

    public onMenuVisibleChange(show: boolean): void {
        this.showMenu = show;
        if (!show) {
            this.profilePanel.reset();
        }
        this.cdr.markForCheck();
    }

    public closeMenu(): void {
        this.showMenu = false;
        this.cdr.markForCheck();
    }

}
