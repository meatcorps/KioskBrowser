import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr"
import {HubConnection, HubConnectionState} from "@microsoft/signalr";
import {environment} from "../../environments/environment";
import {RetryPolicy} from "./retrypolicy";

@Injectable({
  providedIn: 'root'
})
export class PingHubService {
  // @ts-ignore
  private hubConnection: HubConnection;
  private reconnectPolicy: signalR.IRetryPolicy = new RetryPolicy()

  public get connected(): boolean {
    return this.hubConnection.state === HubConnectionState.Connected;
  }

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.url + '/ping')
      .withAutomaticReconnect(this.reconnectPolicy)
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
