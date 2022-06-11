import { Component, OnInit } from '@angular/core';
import {IProductData} from "../../interfaces/IProductData";
import {IGroupData} from "../../interfaces/IGroupData";
import {DataHubService} from "../../hubs/data-hub.service";

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {
  private products: IProductData[] = [];
  public groups: IGroupData[] = [];
  public page: number = 0;
  public position: number = 0;

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

    setInterval(() => this.nextAnimation(), 10000)
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

  public getAnimationClass(index: number): string {
    index = Math.floor(index / 4);

    return index === this.page ? 'fadein' : 'fadeout';
  }

  private nextAnimation() {
    this.page++;
    if (this.page > this.groups.length / 4) {
      this.page = 0;
    }
    setTimeout(() => {
      this.position = -(100 * this.page);
    }, 1000);
  }
}
