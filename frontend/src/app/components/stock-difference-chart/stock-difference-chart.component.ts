import { Component, OnInit, ChangeDetectionStrategy, ViewChild, Injector, ViewContainerRef, ChangeDetectorRef } from '@angular/core';
import * as Excel from "exceljs/dist/exceljs.min.js";
import { NzNotificationService } from 'ng-zorro-antd/notification';
import * as fromCore from 'workstation-core';
import { CHART_METADATA } from './chart-metadata';
import { IStockPerspectiveData } from './common';
import { DepartmentChartComponent } from './department-chart/department-chart.component';
import domtoimage from 'dom-to-image';
import { saveAs } from 'file-saver';

@Component({
    selector: 'app-stock-difference-chart',
    templateUrl: './stock-difference-chart.component.html',
    styleUrls: ['./stock-difference-chart.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class StockDifferenceChartComponent implements OnInit {

    public files: any;
    public datas: Array<IStockPerspectiveData>;
    public departments: Array<string>;
    @ViewChild('chartContainer', { static: true, read: ViewContainerRef })
    private readonly chartContainer: ViewContainerRef;
    @fromCore.LazyService(NzNotificationService)
    private readonly notification: NzNotificationService;
    @fromCore.LazyService(ChangeDetectorRef)
    private readonly cdr: ChangeDetectorRef;
    public constructor(
        protected injector: Injector
    ) { }

    public async ngOnInit(): Promise<void> {
        // const str = localStorage.getItem('analysis');
        // if (str) {
        //     this.datas = JSON.parse(str);
        //     console.log('datas:', this.datas);
        //     this.analyze();
        // }
    }

    public async onFileChange(files: Array<File>): Promise<void> {
        this.notification.info('温馨提示', '数据分析中,请耐心等候');
        this.datas = [];
        const datasArr: Array<Array<IStockPerspectiveData>> = await Promise.all(files.map(file => {
            return new Promise(resolve => {
                var reader = new FileReader();
                reader.onloadend = (event) => {
                    var arrayBuffer = reader.result;
                    var workbook = new Excel.Workbook();
                    workbook.xlsx.load(arrayBuffer).then((workbook) => {
                        const worksheet = workbook.getWorksheet('透视源数据');
                        const datas: Array<IStockPerspectiveData> = [];
                        worksheet.eachRow((row, rowNumber) => {
                            if (rowNumber === 1) { return; }
                            let data: IStockPerspectiveData = {
                                department: row.getCell(1).value,
                                quantityVariance: row.getCell(2).value,
                                skuCount: row.getCell(3).value,
                                storeCount: row.getCell(4).value,
                                quantityAmount: row.getCell(5).value,
                                quantityType: row.getCell(6).value,
                                analysisTime: row.getCell(7).value
                            };
                            datas.push(data);
                        });
                        resolve(datas);
                    });
                };
                reader.readAsArrayBuffer(file);
            });
        })) as any;
        datasArr.forEach(dts => dts.forEach(it => this.datas.push(it)));
        // localStorage.setItem('analysis', JSON.stringify(this.datas));
        await this.analyze();

    }

    public async download(): Promise<void> {
        const nodes: NodeList = document.getElementsByTagName('app-department-chart') as any;
        for (let idx = nodes.length - 1; idx >= 0; idx--) {
            const node: Element = nodes[idx] as any;
            const department = node.getAttribute('department');
            domtoimage.toBlob(node)
                .then(function (blob) {
                    saveAs(blob, `${department}.png`);
                });
        }
    }

    public async analyze(): Promise<void> {
        const set = new Set<string>(this.datas.map(d => d.department));
        this.departments = [];
        set.forEach(d => this.departments.push(d));
        this.renderChart();
        this.cdr.markForCheck();
    }

    private renderChart(): void {
        if (this.chartContainer.length) {
            this.chartContainer.clear();
        }

        this.departments.forEach(d => this.singleDepartmentVisual(d));

        // this.singleDepartmentVisual(this.departments[0]);
    }

    private singleDepartmentVisual(department: string): void {
        const datas = this.datas.filter(d => d.department === department);
        const metadata = {
            datas,
            department
        };
        const ij = Injector.create({
            providers: [
                { provide: CHART_METADATA, useValue: metadata }
            ],
            parent: this.injector
        });
        this.chartContainer.createComponent(DepartmentChartComponent, { injector: ij });
    }

}
