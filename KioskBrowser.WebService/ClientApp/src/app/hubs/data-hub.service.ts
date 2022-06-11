import { Injectable } from '@angular/core';
import {HubConnection, HubConnectionState} from "@microsoft/signalr";
import * as signalR from "@microsoft/signalr";
import {BehaviorSubject, Observable, of, Subject} from "rxjs";
import { IGroupData } from '../interfaces/IGroupData';
import {IProductData} from "../interfaces/IProductData";
import {IMessageData} from "../interfaces/IMessageData";
import {take} from "rxjs/operators";
import {environment} from "../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class DataHubService {
  private hubConnection: HubConnection;
  private groupDataChangeSubject: Subject<IGroupData[]> = new Subject<any>();
  public groupDataChange: Observable<IGroupData[]> = this.groupDataChangeSubject.asObservable();
  private incomingMessageSubject: Subject<IMessageData> = new Subject<any>();
  public incomingMessageChange: Observable<IMessageData> = this.incomingMessageSubject.asObservable();
  private productChangeSubject: Subject<IProductData[]> = new Subject<any>();
  public productDataChange: Observable<IProductData[]> = this.productChangeSubject.asObservable();

  private connectionReadySubject: Subject<void> = new Subject<any>();

  public get connected(): boolean {
    return this.hubConnection.state === HubConnectionState.Connected;
  }

  public get connectionReady(): Observable<void> {
    if (this.connected) {
      return new BehaviorSubject<any>(null);
    }
    return this.connectionReadySubject.pipe(take(1));
  }

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.url + '/data')
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.connectionReadySubject.next();
      })
      .catch(err => {
        console.log('Error while starting connection: ' + err);
      });

    this.hubConnection.on('AllGroups', (data: any) => this.groupDataChangeSubject.next(data));
    this.hubConnection.on('IncomingMessage', (data: any) => this.incomingMessageSubject.next(data));
    this.hubConnection.on('AllProducts', (data: any) => this.productChangeSubject.next(data));
  }

  public addEditGroup(group: IGroupData): Promise<void> {
    return this.hubConnection.invoke(
      "AddEditGroup",
      group.id,
      group.name,
      group.sortIndex);
  }

  public removeGroup(group: IGroupData): Promise<void> {
    return this.hubConnection.invoke("RemoveGroup", group.id);
  }

  public allGroups(): Promise<IGroupData[]> {
    return this.hubConnection.invoke("AllGroups");
  }

  public addEditProduct(product: IProductData): Promise<void> {
    return this.hubConnection.invoke(
      "AddEditProduct",
      product.id,
      product.name,
      product.group,
      product.sortIndex,
      product.totalItems);
  }

  public removeProduct(product: IProductData): Promise<void> {
    return this.hubConnection.invoke("RemoveProduct", product.id);
  }

  public allProduct(): Promise<IProductData[]> {
    return this.hubConnection.invoke("AllProducts");
  }

  public addEditMessage(message: IMessageData): Promise<void> {
    return this.hubConnection.invoke(
      "AddEditMessage",
      message.id,
      message.title,
      message.message);
  }

  public removeMessage(message: IMessageData): Promise<void> {
    return this.hubConnection.invoke("RemoveMessage", message.id);
  }

  public allMessages(): Promise<IGroupData[]> {
    return this.hubConnection.invoke("AllMessages");
  }

  constructor() { }
}

export {
  IGroupData,
  IMessageData,
  IProductData
}
