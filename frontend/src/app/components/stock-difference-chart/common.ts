export enum quantityTypeEnum {
    _南棠ERP可用库存大于亚马逊后台库存 = '_南棠ERP可用库存大于亚马逊后台库存',
    _南棠ERP可用库存小于亚马逊后台库存 = '_南棠ERP可用库存小于亚马逊后台库存'
}

export interface IStockPerspectiveData {
    department: string;
    // 差异数量
    quantityVariance: number;
    skuCount: number;
    // 店铺个数
    storeCount: number;
    // 差异总金额
    quantityAmount: number;
    // 差异类型
    quantityType: quantityTypeEnum;
    // 统计时间
    analysisTime: string;
}