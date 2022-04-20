export interface IMenu {
    key: string;
    title: string;
    url?: string;
    icon?: string;
    open?: boolean;
    selected?: boolean;
    disabled?: boolean;
    children?: Array<IMenu>;
    level?: number;
}
