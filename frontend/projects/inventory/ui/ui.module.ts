import { NgModule } from '@angular/core';
import { UiRoutingModule } from './ui-routing.module';
import { StockAnalysisComponent } from './components/stock-analysis/stock-analysis.component';
import { SharedModule } from '@pucst/shared';

@NgModule({
  declarations: [
    StockAnalysisComponent
  ],
  imports: [
    SharedModule,
    UiRoutingModule
  ],
  exports: [
    StockAnalysisComponent
  ]
})
export class UiModule { }
