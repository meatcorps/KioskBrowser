import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr"
import {HubConnection, HubConnectionState} from "@microsoft/signalr";
import {environment} from "../../environments/environment";
import {RetryPolicy} from "./retrypolicy";

@Injectable({
  providedIn: 'root'
})
export class DataHubService {
  // @ts-ignore
  private hubConnection: HubConnection;
  private reconnectPolicy: signalR.IRetryPolicy = new RetryPolicy()

  constructor() { }

  public get connected(): boolean {
    return this.hubConnection.state === HubConnectionState.Connected;
  }

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.url + '/data')
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

  public async sendMessage(code: string, name: string, message: string): Promise<void> {
    return await this.hubConnection.invoke("SendMessage", code, name, message);
  }

  public async validCode(code: string): Promise<boolean> {
    return await this.hubConnection.invoke("ValidCode", code);
  }

  public async adminCode(code: string): Promise<boolean> {
    return await this.hubConnection.invoke("AdminCode", code);
  }

  public async totalToVerifyMessage(code: string): Promise<number> {
    return await this.hubConnection.invoke("TotalToVerifyMessage", code);
  }

  public async totalToVerifyPicture(code: string): Promise<number> {
    return await this.hubConnection.invoke("TotalToVerifyPicture", code);
  }

  public async pushPublicKey(): Promise<string> {
    return await this.hubConnection.invoke("GetPublicKey");
  }

  public async addPushSubscription(sub: PushSubscription): Promise<void> {
    var subData: any = sub.toJSON();
    console.log(subData.endpoint, subData.keys['auth'], subData.keys['p256dh']);
    await this.hubConnection.invoke("AddPushSubscription", subData.endpoint, subData.keys['p256dh'], subData.keys['auth']);
  }
}
