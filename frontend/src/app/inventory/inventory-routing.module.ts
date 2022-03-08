import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StockAnalysisComponent } from './components/stock-analysis/stock-analysis.component';

const routes: Routes = [
  { path: 'stock-analysis', component: StockAnalysisComponent },
  { path: '', pathMatch: 'full', redirectTo: 'stock-analysis' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InventoryRoutingModule { }
