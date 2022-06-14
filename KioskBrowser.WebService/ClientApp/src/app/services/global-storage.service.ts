import { Injectable } from '@angular/core';
import {DataHubService, IStorageData} from "../hubs/data-hub.service";
import {Observable, Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class GlobalStorageService {
  private storage: IStorageData[] = [];
  private changeReceivedSubject: Subject<string> = new Subject<string>();
  public changeReceived: Observable<string> = this.changeReceivedSubject.asObservable();
  private connectionReadySubject: Subject<string> = new Subject<string>();
  public connectionReady: Observable<string> = this.connectionReadySubject.asObservable();

  constructor(private dataHub: DataHubService) {
    dataHub.connectionReady.subscribe(() => {
      dataHub.allStorage().then(data => {
        this.storage = data;
        this.connectionReadySubject.next();
        console.log(this.storage);
      });
    });
    dataHub.storageChangeChange.subscribe(x => this.changeExternal(x));
    dataHub.storageRemoveChange.subscribe(x => this.removeExternal(x));
  }

  private changeExternal(data: IStorageData): void {
    console.log('changeExternal', data);
    const index = this.storage.findIndex(x => x.id === data.id);
    if (index === -1) {
      this.storage.push(data);
      return;
    }
    this.storage[index] = data;
    this.changeReceivedSubject.next(data.key);
  }

  private removeExternal(data: IStorageData): void {
    const index = this.storage.findIndex(x => x.id === data.id);
    if (index > -1) {
      this.storage.splice(index, 1);
      this.changeReceivedSubject.next(data.key);
    }
  }

  public get(key: string): string {
    const index = this.storage.findIndex(x => x.key === key);
    if (index === -1) {
      return '';
    }
    return this.storage[index].value;
  }

  public getWithDefault(key: string, defaultValue: string): string {
    const value = this.get(key);
    console.log(value);
    if (value === '') {
      this.set(key, defaultValue);
      return defaultValue;
    }
    return value;
  }

  public set(key: string, value: string): void {
    const index = this.storage.findIndex(x => x.key === key);
    const item: IStorageData = {
      id: '',
      key: key,
      value: value
    }
    console.log(key, value, index, this.storage);
    if (index > -1) {
      this.storage[index].value = value;
      item.id = this.storage[index].id;
    }
    this.dataHub.addEditStorage(item);
  }

  public remove(key: string): void {
    const index = this.storage.findIndex(x => x.key === key);
    if (index === -1) {
      return;
    }
    this.dataHub.removeStorage(this.storage[index]);
    this.removeExternal(this.storage[index]);
  }
}
