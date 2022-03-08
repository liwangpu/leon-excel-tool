import { NgModule } from '@angular/core';
import { InventoryRoutingModule } from './inventory-routing.module';
import { SharedModule } from '../shared';
import { StockAnalysisComponent } from './components/stock-analysis/stock-analysis.component';


@NgModule({
  declarations: [
    StockAnalysisComponent
  ],
  imports: [
    SharedModule,
    InventoryRoutingModule
  ]
})
export class InventoryModule { }
