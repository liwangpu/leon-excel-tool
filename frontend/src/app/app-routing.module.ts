import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { standardSubAppRouterGenerator } from 'workstation-shared/starter';
import { AmazonCompensationComponent } from './components/amazon-compensation/amazon-compensation.component';
import { CompensationComponent } from './components/compensation/compensation.component';
import { FreightChargeComponent } from './components/freight-charge/freight-charge.component';
import { HomeComponent } from './components/home/home.component';
import { StockDifferenceAnalysisComponent } from './components/stock-difference-analysis/stock-difference-analysis.component';
import { StockDifferenceChartComponent } from './components/stock-difference-chart/stock-difference-chart.component';
import { TestComponent } from './components/test/test.component';

const routes: Routes = standardSubAppRouterGenerator([
    { path: 'home', component: HomeComponent },
    { path: 'test', component: TestComponent },
    { path: 'compensation', component: CompensationComponent },
    { path: 'amazon-compensation', component: AmazonCompensationComponent },
    { path: 'stock-difference-analysis', component: StockDifferenceAnalysisComponent },
    { path: 'stock-difference-visual', component: StockDifferenceChartComponent },
    { path: 'fregith-charge', component: FreightChargeComponent },
    { path: '', pathMatch: 'full', redirectTo: 'home' }
]);

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
