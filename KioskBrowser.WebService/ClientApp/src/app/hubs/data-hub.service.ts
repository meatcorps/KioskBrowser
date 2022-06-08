import { Injectable } from '@angular/core';
import {HubConnection, HubConnectionState} from "@microsoft/signalr";
import * as signalR from "@microsoft/signalr";
import {Observable, Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class DataHubService {
  private hubConnection: HubConnection;
  private groupDataChangeSubject: Subject<any> = new Subject<any>();
  public groupDataChange: Observable<any> = this.groupDataChangeSubject.asObservable();
  private incomingMessageSubject: Subject<any> = new Subject<any>();
  public incomingMessageChange: Observable<any> = this.incomingMessageSubject.asObservable();
  private productChangeSubject: Subject<any> = new Subject<any>();
  public productDataChange: Observable<any> = this.productChangeSubject.asObservable();

  public get connected(): boolean {
    return this.hubConnection.state === HubConnectionState.Connected;
  }

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5176/data')
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
      })
      .catch(err => {
        console.log('Error while starting connection: ' + err);
      });

    this.hubConnection.on('AllGroups', (data: any) => this.groupDataChangeSubject.next(data));
    this.hubConnection.on('IncomingMessage', (data: any) => this.incomingMessageSubject.next(data));
    this.hubConnection.on('AllProducts', (data: any) => this.productChangeSubject.next(data));
  }

  public addEditGroup(id: string, groupName: string, sortIndex: number): Promise<void> {
    return this.hubConnection.invoke("AddEditGroup", id, groupName, sortIndex);
  }

  public removeGroup(id: string): Promise<void> {
    return this.hubConnection.invoke("RemoveGroup", id);
  }

  public allGroups(): Promise<any> {
    return this.hubConnection.invoke("AllGroups");
  }

  public addEditProduct(id: string, title: string, group: string, sortIndex: number, totalItems: number): Promise<void> {
    return this.hubConnection.invoke("AddEditProduct", id, title, group, sortIndex, totalItems);
  }

  public removeProduct(id: string): Promise<void> {
    return this.hubConnection.invoke("RemoveProduct", id);
  }

  public allProduct(): Promise<any> {
    return this.hubConnection.invoke("AllProducts");
  }

  public addEditMessage(id: string, title: string, message: string): Promise<void> {
    return this.hubConnection.invoke("AddEditMessage", id, title, message);
  }

  public removeMessage(id: string): Promise<void> {
    return this.hubConnection.invoke("RemoveMessage", id);
  }

  public allMessages(): Promise<any> {
    return this.hubConnection.invoke("AllMessages");
  }

  constructor() { }
}
