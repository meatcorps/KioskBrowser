import { Injectable } from '@angular/core';
import {DataHubService, IStorageData} from "../hubs/data-hub.service";

@Injectable({
  providedIn: 'root'
})
export class GlobalStorageService {
  private storage: IStorageData[] = [];

  constructor(private dataHub: DataHubService) {
    dataHub.connectionReady.subscribe(() => {
      dataHub.allStorage().then(data => {
        this.storage = data;
      });
    });
    dataHub.storageChangeChange.subscribe(x => this.changeExternal(x));
    dataHub.storageRemoveChange.subscribe(x => this.removeExternal(x));
  }

  private changeExternal(data: IStorageData): void {
    const index = this.storage.findIndex(x => x.id === data.id);
    if (index > -1) {
      this.storage.push(data);
      return;
    }
    this.storage[index] = data;
  }

  private removeExternal(data: IStorageData): void {
    const index = this.storage.findIndex(x => x.id === data.id);
    if (index > -1) {
      this.storage.splice(index, 1);
    }
  }

  public get(key: string): string {
    const index = this.storage.findIndex(x => x.key === key);
    if (index === -1) {
      return '';
    }
    return this.storage[index].value;
  }

  public set(key: string, value: string): void {
    const index = this.storage.findIndex(x => x.key === key);
    const item: IStorageData = {
      id: '',
      key: key,
      value: value
    }
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
