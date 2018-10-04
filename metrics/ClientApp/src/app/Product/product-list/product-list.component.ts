import { Component, OnInit } from '@angular/core';
import {ProductService} from "../product.service";
import {State} from "@progress/kendo-data-query";
import {DataStateChangeEvent, GridDataResult} from "@progress/kendo-angular-grid";
import {Observable} from "rxjs";

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  public view: Observable<GridDataResult>;
  public state: State = {
    skip: 0,
    take: 50
  };
  constructor(private productService: ProductService) {
    this.view = productService;
    this.productService.query(this.state);
  }

  public dataStateChange(state: DataStateChangeEvent): void {
    this.state = state;
    this.productService.query(state);
  }

  ngOnInit(): void {
  }

}
