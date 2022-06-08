import { Component, OnInit } from '@angular/core';
import {PingHubService} from "./hubs/ping-hub.service";
import {interval, Observable} from "rxjs";
import {filter} from "rxjs/operators";
import {DataHubService} from "./hubs/data-hub.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  title = 'app';
  counter = 0;
  public guid: string = '';

  constructor(public data: DataHubService) {
  }

  ngOnInit(): void {
    this.data.startConnection();
    this.data.groupDataChange.subscribe((data) => console.log(data));
  }

  all(): void {
    this.data.allGroups().then((data) => {
      console.log(typeof data, data);
    });
  }
}
