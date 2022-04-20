import { AbstractType, InjectFlags, InjectionToken, Type } from '@angular/core';
import * as _ from 'lodash';

/**
 * 懒加载服务,使用LazyService装饰器的时候,构造函数必须要注入injector:Injector,名称一模一样
 * @param token token
 * @param notFoundValue notFoundValue
 * @param flags flags
 */
export function LazyService(token: Type<any> | InjectionToken<any> | AbstractType<any>, notFoundValue?: any, flags?: InjectFlags): any {
    return function (target: object, propertyKey: string): any {
        const getter: any = function (): any {
            let service: any = this[`__${propertyKey}`];
            if (!this.injector) {
                console.error(`组件需要使用LazyService装饰器的时候,构造函数必须要注入injector:Injector,名称一模一样`);
                return null;
            }
            if (service === undefined) {
                service = this.injector.get(token, notFoundValue, flags);
                this[`__${propertyKey}`] = service;
            }
            return service;
        };
        Object.defineProperty(target, propertyKey, {
            get: getter,
            enumerable: true
        });
    };
}
