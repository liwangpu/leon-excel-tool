import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { IconDefinition } from '@ant-design/icons-angular';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './components/home/home.component';
import { StarterModule, AppIconSetting } from "workstation-shared/starter";
import { UploaderModule } from "workstation-shared/uploader";
import { AttachmentUploaderModule } from "workstation-shared/attachment-uploader";
import { LAYOUT_STARTUP } from 'workstation-shared/layout';
import * as fromService from './services';
import { CompensationComponent } from './components/compensation/compensation.component';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { TestComponent } from './components/test/test.component';
import { StockDifferenceAnalysisComponent } from './components/stock-difference-analysis/stock-difference-analysis.component';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { StockDifferenceChartComponent } from './components/stock-difference-chart/stock-difference-chart.component';
import { DepartmentChartComponent } from './components/stock-difference-chart/department-chart/department-chart.component';
import { AmazonCompensationComponent } from './components/amazon-compensation/amazon-compensation.component';
import { FreightChargeComponent } from './components/freight-charge/freight-charge.component';
import { AlipayIntelBillCollectComponent } from './components/alipay-intel-bill-collect/alipay-intel-bill-collect.component';

const icons: Array<IconDefinition> = [];

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        CompensationComponent,
        TestComponent,
        StockDifferenceAnalysisComponent,
        StockDifferenceChartComponent,
        DepartmentChartComponent,
        AmazonCompensationComponent,
        FreightChargeComponent,
        AlipayIntelBillCollectComponent
    ],
    imports: [
        StarterModule.forRoot(),
        AppRoutingModule,
        AppIconSetting(icons),
        FormsModule, 
        ReactiveFormsModule,
        NzFormModule,
        NzInputModule,
        UploaderModule,
        AttachmentUploaderModule,
        NzButtonModule,
        NzDatePickerModule,
        NzNotificationModule
    ],
    providers: [
        { provide: LAYOUT_STARTUP, useClass: fromService.LayoutStartupService }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
