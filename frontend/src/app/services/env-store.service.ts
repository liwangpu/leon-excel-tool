import { APP_BASE_HREF } from '@angular/common';
import { Inject, Injectable, isDevMode } from '@angular/core';
import { IEnvStore, IEnvConfig } from '@pucst/core';

@Injectable()
export class EnvStoreService implements IEnvStore {

    public envConfig: IEnvConfig;

    public getEnvConfig(): IEnvConfig {
        return this.envConfig;
    }

    public async loadEnvConfig(): Promise<void> {
        if (this.envConfig) { return; }
        return new Promise((resolve, reject) => {
            let configFileName: string = isDevMode() ? 'env-config.dev.json' : 'env-config.json';
            const configFile: string = `/assets/${configFileName}`;
            let xhr: XMLHttpRequest = new XMLHttpRequest();
            xhr.addEventListener('readystatechange', () => {
                if (xhr.readyState === XMLHttpRequest.DONE) {
                    if (xhr.status == 200) {
                        this.envConfig = JSON.parse(xhr.response);
                    }
                    resolve();
                }
            });
            xhr.open('get', configFile);
            xhr.send();
        });
    }
}
