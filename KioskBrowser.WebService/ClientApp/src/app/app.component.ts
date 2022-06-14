import { Component, OnInit } from '@angular/core';
import {DataHubService} from "./hubs/data-hub.service";
import {NavigationStart, Router} from "@angular/router";
import {filter} from "rxjs/operators";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  title = 'app';
  counter = 0;
  public guid: string = '';
  public currentPage = '';

  constructor(public data: DataHubService, private routing: Router) {
  }

  ngOnInit(): void {
    this.data.startConnection();
    this.data.groupDataChange.subscribe((data) => console.log(data));
    this.routing.events.pipe(
      filter((e) => e instanceof NavigationStart)
    ).subscribe((x: any) => {
      this.currentPage = x.url.replace('/', '');
    });
  }

  all(): void {
    this.data.allGroups().then((data) => {
      console.log(typeof data, data);
    });
  }
}
