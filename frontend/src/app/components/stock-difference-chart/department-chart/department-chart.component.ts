import { Component, OnInit, ChangeDetectionStrategy, Injector, ViewChild, ElementRef, HostBinding } from '@angular/core';
import * as fromCore from 'workstation-core';
import { CHART_METADATA } from '../chart-metadata';
import { Chart } from '@antv/g2';
import { IStockPerspectiveData } from '../common';

@Component({
    selector: 'app-department-chart',
    templateUrl: './department-chart.component.html',
    styleUrls: ['./department-chart.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class DepartmentChartComponent implements OnInit {

    @fromCore.LazyService(CHART_METADATA)
    public readonly metadata: { datas: Array<IStockPerspectiveData>, department: string };
    @ViewChild('containerLeft', { static: true, read: ElementRef })
    private readonly containerLeft: ElementRef;
    @ViewChild('containerRight', { static: true, read: ElementRef })
    private readonly containerRight: ElementRef;
    @HostBinding('attr.department')
    public get department(): string {
        return this.metadata.department;
    }
    public constructor(
        protected injector: Injector
    ) { }

    public ngOnInit(): void {
        // console.log('metadata:',this.metadata);
        this.renderLeftChart();
        this.renderRightChart();
    }

    private renderLeftChart(): void {
        const chart = new Chart({
            container: this.containerLeft.nativeElement,
            // width: 600, // 指定图表宽度
            autoFit: true,
            height: 340, // 指定图表高度,
        });

        chart.data(this.metadata.datas);
        chart.scale({
            analysisTime: {},
            quantityVariance: {
                nice: true,
                alias: '差异数量',
                tickCount: 6,
                type: 'pow',
                // tickMethod: DepartmentChartComponent.getTicks,
            },
        });

        chart.axis('quantityVariance', {
            title: {},
        });

        chart.line()
            .position('analysisTime*quantityVariance')
            .color('quantityType')
            .shape('smooth')
            .label('quantityVariance');

        chart.point()
            .position('analysisTime*quantityVariance')
            .color('quantityType')
            .shape('circle');

        chart.render();
    }

    private renderRightChart(): void {
        const chart = new Chart({
            container: this.containerRight.nativeElement,
            // width: 600, // 指定图表宽度
            autoFit: true,
            height: 340, // 指定图表高度,
        });

        chart.data(this.metadata.datas);
        chart.scale({
            analysisTime: {},
            quantityAmount: {
                nice: true,
                alias: '差异总金额',
                tickCount: 6,
                type: 'pow',
            },
        });

        chart.axis('quantityAmount', {
            title: {},
        });

        chart.line()
            .position('analysisTime*quantityAmount')
            .color('quantityType')
            .shape('smooth')
            .label('quantityAmount');

        chart.point()
            .position('analysisTime*quantityAmount')
            .color('quantityType')
            .shape('circle');

        chart.render();
    }

    private static getTicks(scale) {
        const { min, max, tickCount } = scale;
        console.log('scale:', scale);
        console.log('min:', min);
        console.log('max:', max);
        console.log('tickCount:', tickCount);
        const avg = (max - min) / tickCount;
        const ticks = [];
        for (let i = min; i <= max; i += avg) {
            ticks.push(i);
        }
        return ticks;
    }
}
