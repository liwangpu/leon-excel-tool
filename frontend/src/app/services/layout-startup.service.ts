import { Injectable } from '@angular/core';
import { ILayoutStartup } from 'workstation-shared/layout';
import { IMenu } from 'workstation-shared/layout';

@Injectable()
export class LayoutStartupService implements ILayoutStartup {
    public async getAppName(): Promise<string> {
        return 'Excel小工具';
    }

    public async getMenus(): Promise<Array<IMenu>> {
        return [
            {
                key: 'order',
                title: '订单处理',
                children: [
                    {
                        key: 'compensation',
                        title: '退货赔偿订单处理',
                        url: '/compensation',
                    }
                ]
            },
            {
                key: 'stock',
                title: '库存分析',
                children: [
                    {
                        key: 'compensation',
                        title: '库存差异分析',
                        url: '/stock-difference-analysis',
                    },
                    {
                        key: 'compensation',
                        title: '库存差异可视化',
                        url: '/stock-difference-visual',
                    }
                ]
            },
            // {
            //     key: 'test',
            //     title: '测试',
            //     children: [
            //         {
            //             key: 'test1',
            //             title: 'Websokect测试',
            //             url: '/test',
            //         }
            //     ]
            // }
        ];
    }

    public userProfileRoute(): string {
        return `/mirror/profile`;
    }
}
