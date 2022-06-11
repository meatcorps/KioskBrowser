import { Component, OnInit } from '@angular/core';
import {DataHubService, IGroupData, IProductData} from "../../hubs/data-hub.service";
import {CdkDragDrop} from "@angular/cdk/drag-drop";

@Component({
  selector: 'app-management',
  templateUrl: './management.component.html',
  styleUrls: ['./management.component.css']
})
export class ManagementComponent implements OnInit {
  public products: IProductData[] = [];

  constructor(public data: DataHubService) { }

  ngOnInit(): void {
  }
}
