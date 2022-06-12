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

  constructor(public data: DataHubService) { }

  public ngOnInit(): void {
    this.data.connectionReady.subscribe(() => {
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
    }
    if (this.mode === '+') {
      product.totalItems++;
    }

    this.data.addEditProduct(product).finally(() => {});
  }

}
