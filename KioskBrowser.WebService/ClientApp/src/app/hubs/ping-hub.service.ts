import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr"
import {HubConnection, HubConnectionState} from "@microsoft/signalr";
import {timeout} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class PingHubService {
  private hubConnection: HubConnection;

  public get connected(): boolean {
    return this.hubConnection.state === HubConnectionState.Connected;
  }

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5176/ping')
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
      })
      .catch(err => {
        console.log('Error while starting connection: ' + err);

      });
  }

  public ping() {
    this.hubConnection.invoke("Ping").then(result => {
      console.log('Ping', result);
    });
  }

  public messsage(whatMessage: string) {
    this.hubConnection.invoke("ReturnMessage", whatMessage).then(result => {
      console.log('ReturnMessage', result);
    });
  }

  constructor() { }
}
