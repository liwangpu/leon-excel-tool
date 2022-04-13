import { APP_BASE_HREF } from '@angular/common';
import { Injectable, Injector, isDevMode } from '@angular/core';
import * as fromCore from 'workstation-core';
import { ComponentStore } from '@ngrx/component-store';
import * as _ from 'lodash';
import { Observable } from 'rxjs';
import * as fromModel from '../models';

export interface IStoreState {
    appName: string;
    menus: Array<fromModel.IMenu>;
    menuCollapsed?: boolean;
}

@Injectable()
export class StoreService extends ComponentStore<IStoreState>  {

    public appName$: Observable<string> = this.select(s => s.appName);
    public menus$: Observable<Array<fromModel.IMenu>> = this.select(s => s.menus);
    public menuCollapsed$: Observable<boolean> = this.select(s => s.menuCollapsed);
    @fromCore.LazyService(fromCore.TOKEN_STORE)
    private readonly tokenStore: fromCore.ITokenStore;
    @fromCore.LazyService(APP_BASE_HREF)
    private appBaseHref: string;
    private appStateToolFrame: HTMLIFrameElement;
    private appStateToolFrameContent: Window;
    private handleToolFrameLoadFn: any;
    public constructor(
        protected injector: Injector
    ) {
        super({
            appName: '',
            menus: []
        });
    }

    public async updateAppName(appName: string): Promise<void> {
        this.patchState({ appName });
    }

    public async updateMenus(menus: Array<fromModel.IMenu>): Promise<void> {
        this.patchState({ menus: this.formatMenus(menus) });
    }

    public async toggleMenuCollapsed(): Promise<void> {
        this.patchState({ menuCollapsed: !this.get(s => s.menuCollapsed) });
    }

    private formatMenus(menus: Array<fromModel.IMenu> = []): Array<fromModel.IMenu> {
        menus = _.cloneDeep(menus);
        menus.forEach(m => {
            StoreService.traverseMenu(m, 1);
        });
        return menus;
    }

    private static traverseMenu(menu: fromModel.IMenu, level: number): void {
        menu.level = level;
        menu.open = menu.open === undefined ? true : menu.open;
        if (menu.children?.length) {
            menu.children.forEach(cm => StoreService.traverseMenu(cm, level + 1));
        }
    }
}
