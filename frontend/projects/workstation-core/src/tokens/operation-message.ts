import { InjectionToken } from '@angular/core';

/**
 * 操作消息提示
 */
export interface IOperationMessage {
    /**
     * 操作成功消息提示
     */
    success(message: string): void;
    /**
     * 基础消息提示
     * @param message 消息
     */
    info(message: string): void;
    /**
     * 警告消息提示
     * @param message 消息
     */
    warn(message: string): void;
    /**
     * 错误消息提示
     * @param message 消息
     */
    error(message: string): void;
}

export const OPERATION_MESSAGE: InjectionToken<IOperationMessage> = new InjectionToken<IOperationMessage>('Operation Message');
