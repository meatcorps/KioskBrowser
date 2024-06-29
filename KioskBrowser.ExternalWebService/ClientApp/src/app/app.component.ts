import {Component, OnInit} from '@angular/core';
import {PingHubService} from "./hubs/ping-hub.service";
import {filter, interval, timer} from "rxjs";
import {TransferHubService} from "./hubs/transfer-hub.service";
import {CodeService} from "./services/code.service";
import {DataHubService} from "./hubs/data-hub.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit{
  title = 'app';
  public imageData: string = "";
  public userCodeInput: string = "";
  public errorMessage: string = "";

  constructor(private pingHub: PingHubService, private transferHub: TransferHubService, public dataHub: DataHubService, public codeService: CodeService) {
  }

  public ngOnInit(): void {
    this.pingHub.startConnection();
    this.transferHub.startConnection();
    this.dataHub.startConnection();
  }

  public async enterCode(userCodeInput: string) {
    await this.codeService.setCode(userCodeInput.toUpperCase());
    this.errorMessage = "Invalid code! Try again";
    this.userCodeInput = "";
    setTimeout(() => this.errorMessage = "", 5000);
  }
}
