import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';

// @dynamic
export function topicFilter(topic: string): any {
    return filter((x: { topic: string; data: any }) => x.topic === topic);
}

// @dynamic
export function topicFilters(...topics: Array<string>): any {
    return filter((x: { topic: string; data: any }) => topics.indexOf(x.topic) > -1);
}

// @dynamic
export function topicExpressionFilter(expression: (topic: string) => boolean): any {
    return filter((x: { topic: string; data: any }) => expression(x.topic));
}

// @dynamic
export const dataMap: any = map((ms: { topic: string; data?: any }) => ms.data);

// @dynamic
export const topicMap: any = map((ms: { topic: string; data?: any }) => ms.topic);

export interface IMessageOpsat {
    message$: Observable<{ topic: string; data: any }>;
    publish(topic: string, data?: any): void;
}

export const APP_MESSAGE_OPSAT: InjectionToken<IMessageOpsat> = new InjectionToken<IMessageOpsat>('app message opsat');
