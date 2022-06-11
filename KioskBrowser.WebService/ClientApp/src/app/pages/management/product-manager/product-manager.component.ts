import { Component, OnInit } from '@angular/core';
import {IProductData} from "../../../interfaces/IProductData";
import {DataHubService, IGroupData} from "../../../hubs/data-hub.service";
import {CdkDragDrop} from "@angular/cdk/drag-drop";

@Component({
  selector: 'app-product-manager',
  templateUrl: './product-manager.component.html',
  styleUrls: ['./product-manager.component.css']
})
export class ProductManagerComponent implements OnInit {

  public groups: IGroupData[] = [];
  public products: IProductData[] = [];
  public currentProduct: IProductData = {id: '', title: '', group: '', sortIndex: 0, totalItems: 1};

  constructor(public data: DataHubService) { }

  ngOnInit(): void {
    this.data.connectionReady.subscribe(() => {
      console.log('Ready');
      this.data
        .allProduct()
        .then(x => this.setProductCollection(x));

      this.data
        .allGroups()
        .then(x => this.setGroupCollection(x));

      this.data.productDataChange.subscribe(x => this.setProductCollection(x));
    });
  }

  public setProductCollection(products: IProductData[]) {
    this.products = products
      .sort((a, b) => a.sortIndex > b.sortIndex ? 1 : -1);

    for (let i = 0; i < this.products.length; i++) {
      this.products[i].sortIndex = i;
    }
  }

  public setGroupCollection(groups: IGroupData[]) {
    this.groups = groups
      .sort((a, b) => a.sortIndex > b.sortIndex ? 1 : -1);
  }

  public resetCurrentProduct() {
    this.currentProduct = {id: '', title: '', group: '', sortIndex: 0, totalItems: 1};
  }

  public saveCurrentProduct() {
    this.data.addEditProduct(this.currentProduct).finally(() => {
      alert('Done');
    });
    this.resetCurrentProduct();
  }

  public removeProduct(product: IProductData) {
    if (!confirm('Are you sure?')) {
      return;
    }
    this.data.removeProduct(product).finally(() => alert('Done'));
  }

  public productDrop(event: CdkDragDrop<any, any>) {
    this.products[event.previousIndex].sortIndex = event.currentIndex;
    this.products[event.currentIndex].sortIndex = event.previousIndex;

    this.data.addEditProduct(this.products[event.previousIndex]).finally(() => {});
    this.data.addEditProduct(this.products[event.currentIndex]).finally(() => {});
  }

  public getGroupNameFromId(groupId: string) {
    const index = this.groups.findIndex(x => x.id === groupId);
    if (index === -1) {
      return 'Unknown';
    }
    return this.groups[index].name;
  }


}
