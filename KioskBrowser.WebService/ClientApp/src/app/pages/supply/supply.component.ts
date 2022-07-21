import { Component, OnInit } from '@angular/core';
import {IProductData} from "../../interfaces/IProductData";
import {IGroupData} from "../../interfaces/IGroupData";
import {DataHubService} from "../../hubs/data-hub.service";

@Component({
  selector: 'app-supply',
  templateUrl: './supply.component.html',
  styleUrls: ['./supply.component.css']
})
export class SupplyComponent implements OnInit {
  private products: IProductData[] = [];
  public groups: IGroupData[] = [];
  public page: number = 0;
  public position: number = 0;
  public mode: string = "-";
  public currentProduct: IProductData = {id: '', group: '', sortIndex: 0, name: '', totalItems: 0};

  constructor(public data: DataHubService) { }

  public ngOnInit(): void {
    this.data.connectionReady.subscribe(() => {
      console.log('Ready');
      this.data
        .allProduct()
        .then(x => this.setProductCollection(x));

      this.data
        .allGroups()
        .then(x => this.setGroupCollection(x));

      this.data.productDataChange.subscribe(x => this.setProductCollection(x));
      this.data.groupDataChange.subscribe(x => this.setGroupCollection(x));
    });
  }

  public setGroupCollection(groups: IGroupData[]) {
    this.groups = groups
      .sort((a, b) => a.sortIndex > b.sortIndex ? 1 : -1);
  }

  public setProductCollection(products: IProductData[]) {
    this.products = products
      .sort((a, b) => a.sortIndex > b.sortIndex ? 1 : -1);
  }

  public allProductsFromGroup(group: IGroupData): IProductData[] {
    return this.products.filter(x => x.group === group.id);
  }

  public changeSupply(product: IProductData) {
    if (this.mode === '-' && product.totalItems > 0) {
      product.totalItems--;
      this.data.addEditProduct(product).finally(() => {});
    }
    if (this.mode === '+') {
      product.totalItems++;
      this.data.addEditProduct(product).finally(() => {});
    }
    this.currentProduct = product;
  }

  public addCurrentProduct() {
    this.currentProduct.id = '';
    this.saveCurrentProduct();
  }

  public saveCurrentProduct() {
    if (this.currentProduct.id === '') {
      this.currentProduct.sortIndex = this.products.length + 1;
    }
    this.data.addEditProduct(this.currentProduct).finally(() => {});
    this.resetCurrentProduct();
  }

  public resetCurrentProduct() {
    this.currentProduct = {id: '', group: '', sortIndex: 0, name: '', totalItems: 0};
  }

  public removeCurrentProduct() {
    if (!confirm('Are you sure?')) {
      return;
    }
    this.data.removeProduct(this.currentProduct).finally(() => alert('Done'));
    this.resetCurrentProduct();
  }

}
