import { DatePipe } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { AppComponent } from '../app.component';
import { AuthService } from '../Services/auth.service';
import { ToastrService } from '../Services/toastr.service';
declare var Window: any;
declare var $: any;

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.scss']
})
export class CustomerComponent implements OnInit {

  dataTable: any;
  dtOptions: any;
  tableData: any;
  @ViewChild('dataTable', { static: true }) table: any;
  @ViewChild('btnClose', { static: true })
  btnClose!: ElementRef;
  @ViewChild('btnModalOpen', { static: true })
  btnModalOpen!: ElementRef;
  customerForm!: FormGroup;
  customerModel: any;
  parent = [];
  header!: string;

  constructor(private authService: AuthService, private fb: FormBuilder, private router: Router, private toastrService: ToastrService, private appComponent: AppComponent) {
    Window['appCus'] = this;
   }

   ngOnInit() {
    this.getCustomerList();
    this.CreateCustomerForm();
  }

  CreateCustomerForm() {
    this.header = 'Create Customer';
    this.customerForm = this.fb.group({
      VCustomerId: ['', Validators.nullValidator],
      VCustomerFirstName: ['', Validators.required],
      VCustomerLastName: ['', Validators.required],
      VCustomerAddress: ['', Validators.required]
    });
  }

  getCustomerList() {
    this.authService.getCustomerList().subscribe(data => {
      data.customerlist.forEach((value: any, index: any) => {
        value.index = index + 1;
      });
      this.tableData = data.customerlist;
      this.dtOptions = {
        data: this.tableData,
        columns: [
          {title: '#', data: 'index'},
          {title: 'Customer FirstName', data: 'vCustomerFirstName'},
          {title: 'Customer LastName', data: 'vCustomerLastName'},
          {title: 'Customer Address', data: 'vCustomerAddress'},
          {
            title: 'Action',
            data: 'vCustomerId',
            sortable: false,
            'render': function (data: any, type: any, full: any, meta: any) {
                const bid = '\'' + full.vCustomerId + '\'';
                return '<div class="text-center fs-18"> ' +
                    '<a title="Edit" onclick="Window.appCus.edit(' + bid + ')"><i class="far fa-edit" style="cursor: pointer;"> </i> </a> |' +
                    '<a title="Delete" onclick="Window.appCus.remove(' + bid + ')"> <i class="fas fa-trash-alt" style="color: red; cursor: pointer;"></i></a>' +
                    '</div>';
            }
          }
        ],
        responsive: true,
        autoWidth: false
      };
    }, err => {
      console.log(err);
    }, () => {
      this.dataTable = $(this.table.nativeElement);
      this.dataTable.DataTable(this.dtOptions);
    });
  }

  saveCustomer() {
    if (this.customerForm.valid) {
      this.customerModel = Object.assign({}, this.customerForm.value);
      this.customerModel.VCustomerId = (this.customerModel.VCustomerId === '') ? null : this.customerModel.VCustomerId;
      this.authService.saveCustomer(this.customerModel).subscribe(() => {
        this.btnClose.nativeElement.click();
        this.toastrService.success('Success', 'Customer save successfully');
        const url = this.router.url;
        window.location.reload();
        this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
          this.router.navigate([url]);
        });
      }, error => {
        this.toastrService.error('Failed', 'Customer save failed');
        console.log(error);
      });
    } else {
      this.appComponent.validateAllFormFields(this.customerForm);
    }
  }

  edit(data: any) {
    this.btnModalOpen.nativeElement.click();
    this.header = 'Update Customer';
      const customer = this.tableData.filter((x: { vCustomerId: any; }) => x.vCustomerId === data)[0];
      this.customerForm = this.fb.group({
        VCustomerId: [customer.vCustomerId, Validators.required],
        VCustomerFirstName: [customer.vCustomerFirstName, Validators.required],
        VCustomerLastName: [customer.vCustomerLastName, Validators.required],
        VCustomerAddress: [customer.vCustomerAddress, Validators.required]
      });
  }

  remove(data: any) {
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
        this.authService.deleteCustomer(data).subscribe(() => {
          this.toastrService.success('Success', 'Customer delete successfully');
          const url = this.router.url;
          window.location.href = url;
        }, error => {
          this.toastrService.error('Error', 'Customer delete failed');
          console.log(error);
        });
      }
    });
  }

  Create() {
    this.CreateCustomerForm();
  }

  Close() {
    this.CreateCustomerForm();
  }
}