import { InjectionToken } from '@angular/core';
import { IMenu } from '../models/i-menu';

export interface ILayoutStartup {
    getAppName(): Promise<string>;
    getMenus(): Promise<Array<IMenu>>;
    userProfileRoute?(): string;
}

export const LAYOUT_STARTUP: InjectionToken<ILayoutStartup> = new InjectionToken<ILayoutStartup>('layout startup');
