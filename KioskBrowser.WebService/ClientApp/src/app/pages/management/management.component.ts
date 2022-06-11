import { Component, OnInit } from '@angular/core';
import {DataHubService, IGroupData, IProductData} from "../../hubs/data-hub.service";

@Component({
  selector: 'app-management',
  templateUrl: './management.component.html',
  styleUrls: ['./management.component.css']
})
export class ManagementComponent implements OnInit {
  public groups: IGroupData[] = [];
  public products: IProductData[] = [];

  constructor(public data: DataHubService) { }

  ngOnInit(): void {
    this.data.connectionReady.subscribe(() => {
      this.data
        .allGroups()
        .then(x => this.groups = x);

      this.data
        .allProduct()
        .then(x => this.products = x);

      this.data.groupDataChange.subscribe(x => this.groups = x);
      this.data.productDataChange.subscribe(x => this.products = x);
    });
  }

}
