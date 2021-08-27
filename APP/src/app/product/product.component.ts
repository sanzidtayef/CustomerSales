import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { AppComponent } from '../app.component';
import { AuthService } from '../Services/auth.service';
import { ToastrService } from '../Services/toastr.service';
import { LoaderService } from '../Services/loader.service';
declare var $: any;
declare var Window: any;

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss']
})
export class ProductComponent implements OnInit {

  dataTable: any;
  dtOptions: any;
  tableData: any;
  @ViewChild('dataTable', { static: true })
  table!: { nativeElement: any; };
  @ViewChild('btnClose', { static: true })
  btnClose!: ElementRef;
  @ViewChild('btnModalOpen', { static: true })
  btnModalOpen!: ElementRef;
  productForm!: FormGroup;
  productModel: any;
  parent = [];
  header!: string;


  constructor(private authService: AuthService, private loaderservice: LoaderService, private fb: FormBuilder, private router: Router, private toastrService: ToastrService, private appComponent: AppComponent) {
    Window['apppro'] = this;
   }

   ngOnInit() {
    this.getProductList();
    this.CreateProductForm();
  }

  CreateProductForm() {
    this.header = 'Create Customer';
    this.productForm = this.fb.group({
      VProductId: ['', Validators.nullValidator],
      VProductName: ['', Validators.required],
      NProductPrice: ['', Validators.required]
    });
  }

  getProductList() {
    this.authService.getProductList().subscribe(data => {
      data.productlist.forEach((value: any, index: any) => {
        value.index = index + 1;
      });
      this.tableData = data.productlist;
      this.dtOptions = {
        data: this.tableData,
        columns: [
          {title: '#', data: 'index'},
          {title: 'Product Name', data: 'vProductName'},
          {title: 'Product Price', data: 'nProductPrice'},
          {
            title: 'Action',
            data: 'vProductId',
            sortable: false,
            'render': function (data: any, type: any, full: any, meta: any) {
                const bid = '\'' + full.vProductId + '\'';
                return '<div class="text-center fs-18"> ' +
                    '<a title="Edit" class="edit" onclick="Window.apppro.edit(' + bid + ')"><i class="far fa-edit" style="cursor: pointer;"> </i> </a>' +
                    '| <a title="Delete" onclick="Window.apppro.remove(' + bid + ')"> <i class="fas fa-trash-alt" style="color: red; cursor: pointer;"></i></a>' +
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

  saveProduct() {
    if (this.productForm.valid) {
      this.productModel = Object.assign({}, this.productForm.value);
      
      this.productModel.VProductId = (this.productModel.VProductId === '') ? null : this.productModel.VProductId;
      this.authService.saveProduct(this.productModel).subscribe(() => {
        this.btnClose.nativeElement.click();
        this.toastrService.success('Success', 'Product save successfully');
        const url = this.router.url;
        window.location.reload();
        this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
          this.router.navigate([url]);
        });
      }, error => {
        this.toastrService.error('Failed', 'Product save failed');
        console.log(error);
      });
    } else {
      this.appComponent.validateAllFormFields(this.productForm);
    }
  }

  edit(data: any) {
    this.btnModalOpen.nativeElement.click();
    this.header = 'Update Product';
      const product = this.tableData.filter((x: { vProductId: any; }) => x.vProductId === data)[0];
      this.productForm = this.fb.group({
        VProductId: [product.vProductId, Validators.required],
        VProductName: [product.vProductName, Validators.required],
        NProductPrice: [product.nProductPrice, Validators.required]
      });
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
        this.authService.deleteProduct(id).subscribe(() => {
          this.toastrService.success('Success', 'Product delete successfully');
          const url = this.router.url;
          window.location.href = url;
        }, error => {
          this.toastrService.error('Error', 'Product delete failed');
          console.log(error);
        });
      }
    });
  }

  Create() {
    this.CreateProductForm();
  }

  Close() {
    this.CreateProductForm();
  }
}
