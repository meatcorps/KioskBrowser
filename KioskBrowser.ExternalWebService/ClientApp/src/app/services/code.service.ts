import { Injectable } from '@angular/core';
import {DataHubService} from "../hubs/data-hub.service";
import {filter, interval, pipe} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class CodeService {
  private oldCode: string = "";

  constructor(private dataHub: DataHubService) {
    interval(1000).pipe(
      filter(() => this.dataHub.connected && this.oldCode !== this.currentCode())
    ).subscribe(() => this.setCode(this.currentCode()))
  }

  public CodeValid(): boolean {
    return this.currentCode().length === 4;
  }

  public currentCode(): string {
      if (localStorage.getItem("code") === null)
        localStorage.setItem("code", "");

      return localStorage.getItem("code")!;
  }

  public async setCode(code: string): Promise<void> {
      const isValid = await this.dataHub.validCode(code);

      if (!isValid) {
        localStorage.setItem("code", "");
        this.oldCode = "";
        return;
      }

      this.oldCode = code;
      localStorage.setItem("code", code);
  }
}
