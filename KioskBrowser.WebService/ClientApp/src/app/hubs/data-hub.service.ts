import {Injectable} from '@angular/core';
import {HubConnection, HubConnectionState} from "@microsoft/signalr";
import * as signalR from "@microsoft/signalr";
import {BehaviorSubject, Observable, Subject} from "rxjs";
import {IGroupData} from '../interfaces/IGroupData';
import {IProductData} from "../interfaces/IProductData";
import {IMessageData} from "../interfaces/IMessageData";
import {IStorageData} from "../interfaces/IStorageData";
import {take} from "rxjs/operators";
import {environment} from "../../environments/environment";
import { RetryPolicy } from './retrypolicy';
import { HttpClient } from '@angular/common/http';

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
  private storageChangeSubject: Subject<IStorageData> = new Subject<any>();
  public storageChangeChange: Observable<IStorageData> = this.storageChangeSubject.asObservable();
  private storageRemoveSubject: Subject<IStorageData> = new Subject<any>();
  public storageRemoveChange: Observable<IStorageData> = this.storageRemoveSubject.asObservable();
  private newPhotoSubject: Subject<string> = new Subject<any>();
  public newPhoto: Observable<string> = this.newPhotoSubject.asObservable();

  private connectionReadySubject: Subject<void> = new Subject<any>();
  private reconnectPolicy: signalR.IRetryPolicy = new RetryPolicy()

  public get connected(): boolean {
    if (!this.hubConnection) {
      return false;
    }
    return this.hubConnection.state === HubConnectionState.Connected;
  }

  public get connectionReady(): Observable<void> {
    if (this.connected) {
      return new BehaviorSubject<any>(null);
    }
    return this.connectionReadySubject.pipe(take(1));
  }

  constructor(private http: HttpClient) {
  }

  public startConnection() {
    if (this.connected) {
      return;
    }
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.url + '/data')
      .withAutomaticReconnect(this.reconnectPolicy)
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.connectionReadySubject.next();
      })
      .catch(err => {
        console.log('Error while starting connection: ' + err);
        setTimeout(() => this.startConnection(), 1000);
      });

    this.hubConnection.on('AllGroups', (data: any) => this.groupDataChangeSubject.next(data));
    this.hubConnection.on('IncomingMessage', (data: any) => this.incomingMessageSubject.next(data));
    this.hubConnection.on('AllProducts', (data: any) => this.productChangeSubject.next(data));
    this.hubConnection.on('StorageChange', (data: any) => this.storageChangeSubject.next(data));
    this.hubConnection.on('StorageRemove', (data: any) => this.storageRemoveSubject.next(data));
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

  public allMessages(): Promise<IMessageData[]> {
    return this.hubConnection.invoke("AllMessages");
  }

  public addEditStorage(storageItem: IStorageData): Promise<void> {
    return this.hubConnection.invoke(
      "AddEditStorage",
      storageItem.id,
      storageItem.key,
      storageItem.value);
  }

  public removeStorage(storageItem: IStorageData): Promise<void> {
    return this.hubConnection.invoke("RemoveStorage", storageItem.id);
  }

  public allStorage(): Promise<IStorageData[]> {
    return this.hubConnection.invoke("AllStorage");
  }

  public startListenForPhotos(): Observable<string> {
    this.hubConnection.invoke("StartPhotoStream");
    this.hubConnection.on('newPhoto', data => {
      this.newPhotoSubject.next(data);
    });
    return this.newPhoto;
  }

  public httpGetAllGroups(): Observable<IGroupData[]> {
    var data: any = this.http
    .get(environment.url + '/Supply/GetGroups/').pipe(take(1));
    return data;
  }

  public httpGetAllProducts(): Observable<IProductData[]> {
    var data: any = this.http
    .get(environment.url + '/Supply/GetProducts/').pipe(take(1));
    return data;
  }

  public httpAddSupplyProduct(product: IProductData): Observable<IProductData> {
    var data: any = this.http
    .get(environment.url + '/Supply/Products/Add/' + product.id).pipe(take(1));
    return data;
  }

  public httpRemoveSupplyProduct(product: IProductData): Observable<IProductData> {
    var data: any = this.http
    .get(environment.url + '/Supply/Products/Subtract/' + product.id).pipe(take(1));
    return data;
  }

}

export {
  IGroupData,
  IMessageData,
  IProductData,
  IStorageData
}
