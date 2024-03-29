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
                        title: '退货赔偿订单',
                        url: '/compensation',
                    },
                    {
                        key: 'compensation',
                        title: '亚马逊索赔',
                        url: '/amazon-compensation',
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
                ]
            },
            {
                key: 'stock',
                title: '运费处理',
                children: [
                    {
                        key: 'compensation',
                        title: '空海运差异分析',
                        url: '/fregith-charge',
                    }
                ]
            },
            {
              key: 'aliExpress',
              title: '速卖通',
              children: [
                  {
                      key: 'alipay-intel-bill-collect',
                      title: '支付宝国际账单汇总',
                      url: '/alipay-intel-bill-collect',
                  }
              ]
          },
        ];
    }

    public userProfileRoute(): string {
        return `/mirror/profile`;
    }
}
