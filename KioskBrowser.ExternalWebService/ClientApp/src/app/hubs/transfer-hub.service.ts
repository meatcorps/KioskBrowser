// noinspection UnnecessaryLocalVariableJS

import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr"
import {HubConnection, HubConnectionState} from "@microsoft/signalr";
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {lastValueFrom} from "rxjs";
import {RetryPolicy} from "./retrypolicy";

@Injectable({
  providedIn: 'root'
})
class TransferHubService {
  // @ts-ignore
  private hubConnection: HubConnection;
  private reconnectPolicy: signalR.IRetryPolicy = new RetryPolicy()

  public get connected(): boolean {
    return this.hubConnection.state === HubConnectionState.Connected;
  }

  constructor(private http: HttpClient) { }

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.url + '/transfer')
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

  public async chunkSize(): Promise<number> {
    return await this.hubConnection.invoke("ChunkSize");
  }

  public async totalChunks(id: string): Promise<number> {
    return await this.hubConnection.invoke("TotalChunks", id);
  }

  public async transferRequest(code: string, name: string, size: number, metaData: string): Promise<string> {
    return await this.hubConnection.invoke("TransferRequest", code, name, size, metaData);
  }

  public async sendData(id: string, chunkNr: number, data: string): Promise<number> {
    const result = await lastValueFrom(this.http.post(environment.url + '/transfer/upload/' + id + '/' + chunkNr, data));
    return parseInt(result.toString());
  }

  public async getNextVerifyObject(id: string, admin: string, type: string): Promise<INextToVerifyObject> {
    const result: any = await lastValueFrom(this.http.get(environment.url + '/transfer/verify/' + type + '/' + admin + '/' + id));
    return result;
  }

  public async getVerificationForObject(id: string, admin: string, type: string, accept: boolean): Promise<INextToVerifyObject> {
    const result: any = await lastValueFrom(this.http.get(environment.url + '/transfer/' + (accept ? 'accept' : 'decline') + '/' + type + '/' + admin + '/' + id));
    return result;
  }
}

interface INextToVerifyObject {
  total: number,
  object: IVerifyObject
}

interface IVerifyObject {
  Id: string;
  Type: string;
  Who: string;
  Message: string;
  Data: string;
}

export { TransferHubService, INextToVerifyObject, IVerifyObject };
