import { InjectionToken } from '@angular/core';
import { IEnvConfig } from '../models/i-env-config';

export interface IEnvStore {
    getEnvConfig(): IEnvConfig;
    loadEnvConfig(): Promise<void>;
}

export const ENV_STORE: InjectionToken<IEnvStore> = new InjectionToken<IEnvStore>('env store');
