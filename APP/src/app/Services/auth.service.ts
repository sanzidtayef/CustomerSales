import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  

  private baseUrl = environment.apiUrl;
  private customerUrl = this.baseUrl +  'Customer/';
  private productUrl = this.baseUrl +  'Product/';
  private saleUrl = this.baseUrl +  'Sale/';

  constructor(private http: HttpClient, private router: Router,) { }

    // Customer
    getCustomerList(): Observable<any> {
      return this.http.get<any>(this.customerUrl + 'getCustomerList');
    }
  
    saveCustomer(model: any) {
      return this.http.post(this.customerUrl + 'saveCustomer', model);
    }

    deleteCustomer(id: string) {
      return this.http.get(this.customerUrl + 'deleteCustomer?Id=' + id);
    }

    // Product
    getProductList(): Observable<any> {
      return this.http.get<any>(this.productUrl + 'getProductList');
    }

    saveProduct(model: any) {
      return this.http.post(this.productUrl + 'saveProduct', model);
    }

    deleteProduct(id: any) {
      return this.http.get(this.productUrl + 'deleteProduct?Id=' + id);
    }

    // Sales
    getSaleList(): Observable<any> {
      return this.http.get<any>(this.saleUrl + 'getSaleList');
    }

    saveSale(model: any) {
      return this.http.post(this.saleUrl + 'saveSale', model);
    }

    getSaleById(id: any) {
      return this.http.get<any>(this.saleUrl + 'getSaleById?Id=' + id);
    }

    deleteSale(id: any) {
      return this.http.get(this.saleUrl + 'deleteSale?Id=' + id);
    }
}