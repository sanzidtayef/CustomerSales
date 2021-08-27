import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppheaderComponent } from './appheader/appheader.component';
import { AppfooterComponent } from './appfooter/appfooter.component';
import { CustomerComponent } from './customer/customer.component';
import { ProductComponent } from './product/product.component';
import { SalesComponent } from './sales/sales.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';

const routes: Routes = [
      { path: '', component: CustomerComponent},
      { path: 'pro', component: ProductComponent},
      { path: 'sal', component: SalesComponent},

      {path: '**', component: PageNotFoundComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
