import { DatePipe } from '@angular/common';
import { ElementRef, ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AppComponent } from '../app.component';
import { AuthService } from '../Services/auth.service';
import { LoaderService } from '../Services/loader.service';
import { ToastrService } from '../Services/toastr.service';
import { Select2OptionData } from 'ng-select2';
import Swal from 'sweetalert2';
declare var $: any;
declare var Window: any;

@Component({
  selector: 'app-sales',
  templateUrl: './sales.component.html',
  styleUrls: ['./sales.component.scss']
})
export class SalesComponent implements OnInit {

  isDataGridShow = true;
  isBtnCreateShow = true;
  dataTable: any;
  dtOptions: any;
  tableData = [];
  @ViewChild('dataTable', { static: true })
  table!: { nativeElement: any; };
  @ViewChild('btnEditShow', { static: true })
  btnEditShow!: ElementRef;
  saleForm!: FormGroup;
  saleModel: any;
  customers: Array<Select2OptionData> = [];
  products: Array<Select2OptionData> = [];

  constructor(private authService: AuthService, private datepipe: DatePipe, private router: Router, private appComponent: AppComponent, private fb: FormBuilder, private toastrService: ToastrService, private loaderservice: LoaderService) {
    Window['appSal'] = this;
   }

  ngOnInit() {
    this.CreateSaleForm();
    this.isDataGridShow = true;
    this.GetSaleList();
  }

  CreateSaleForm() {
    this.saleForm = this.fb.group({
      VSalesId: ['', Validators.nullValidator],
      VCustomerId: ['', Validators.required],
      DDate: ['', Validators.required],
      NInvoiceNo: ['', Validators.required],
      SalesDetails: this.fb.array([])
    });
    this.AddSaleDetail('');
  }

  get SalesDetails(): FormArray {
    return this.saleForm.get('SalesDetails') as FormArray;
  }

  AddSaleDetail(sd: any) {
    this.SalesDetails.push(this.fb.group({
      VSalesDetailsId: [sd.vSalesDetailsId, Validators.nullValidator],
      VProductId: [sd.vProductId, Validators.required],
      NQuantity: [sd.nQuantity, Validators.required],
      NProductPrice: [sd.nProductPrice, Validators.required]
    }));
  }

  RemoveSaleDetail(index: any) {
    this.SalesDetails.removeAt(index);
  }

  Create() {
    this.CreateSaleForm();
    this.isDataGridShow = false;
    this.isBtnCreateShow = false;
  }

  Close() {
    this.isDataGridShow = true;
    this.isBtnCreateShow = true;
  }

  GetSaleList() {
    this.authService.getSaleList().subscribe(data => {
      data.salelist.forEach((value: { index: any; }, index: number) => {
        value.index = index + 1;
      });
      this.tableData = data.salelist;
      this.dtOptions = {
        data: this.tableData,
        columns: [
          {title: '#', data: 'index'},
          {title: 'Customer First Name', data: 'vCustomerFirstName'},
          {title: 'Issued Date', data: 'dDate'},
          {title: 'Invoice No', data: 'nInvoiceNo'},
          {
            title: 'Action',
            data: 'vSalesId',
            sortable: false,
            'render': function (data: any, type: any, full: any, meta: any) {
                const bid = '\'' + full.vSalesId + '\'';
                return '<div class="text-center fs-18"> ' +
                    '<a title="Edit" class="edit" onclick="Window.appSal.edit(' + bid + ')"><i class="far fa-edit" style="cursor: pointer;"> </i> </a>' +
                    '| <a title="Delete" onclick="Window.appSal.remove(' + bid + ')"> <i class="fas fa-trash-alt" style="color: red; cursor: pointer;"></i></a>' +
                    '</div>';
            }
          }
        ],
        responsive: true,
        autoWidth: false
      };
      this.customers = [];
      data.customerlist.forEach((customer: any) => {
        this.customers.push({
          id: customer.vCustomerId,
          text: customer.vCustomerFirstName
        });
      });
      this.products = [];
      data.productlist.forEach((product: any) => {
        this.products.push({
          id: product.vProductId,
          text: product.vProductName
        });
      });
    }, err => {
      console.log(err);
    }, () => {
      this.dataTable = $(this.table.nativeElement);
      this.dataTable.DataTable(this.dtOptions);
    });
  }

  SaveSale() {
    if (this.saleForm.valid) {
      this.saleModel = Object.assign({}, this.saleForm.value);
      this.saleModel.VSalesId = (this.saleModel.VSalesId === '') ? null : this.saleModel.VSalesId;
      this.authService.saveSale(this.saleModel).subscribe(() => {
        this.toastrService.success('', 'Data save successfully');
        this.isDataGridShow = true;
        this.isBtnCreateShow = true;
        window.location.reload();
      }, (error: any) => {
        this.toastrService.error('', 'Data save failed');
        console.log(error);
      });
    } else {
      this.appComponent.validateAllFormFields(this.saleForm);
    }
  }

  edit(id: any) {
    this.authService.getSaleById(id).subscribe((data: { sale: any[]; }) => {
      const sale = data.sale[0];
      this.saleForm = this.fb.group({
        VSalesId: [sale.vSalesId, Validators.nullValidator],
        DDate: [sale.dDate, Validators.required],
        VCustomerId: [sale.vCustomerId, Validators.required],
        NInvoiceNo: [sale.nInvoiceNo, Validators.required],
        SalesDetails: this.fb.array([])
      });

      sale.salesDetail.forEach((sd: any) => {
        this.AddSaleDetail(sd);
      });
      this.btnEditShow.nativeElement.click();
    }, (error: any) => {
      console.log(error);
    });
  }

  EditShow() {
    this.isDataGridShow = false;
    this.isBtnCreateShow = false;
  }

  remove(id: any) {
    Swal.fire({
      title: 'Are you sure to delete this?',
      text: 'You won\'t be able to revert this!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
      if (result.value) {
        this.authService.deleteSale(id).subscribe(() => {
          this.toastrService.success('Success', 'Sales delete successfully');
          const url = this.router.url;
          window.location.href = url;
        }, error => {
          this.toastrService.error('Error', 'Sales delete failed');
          console.log(error);
        });
      }
    });
  }
}