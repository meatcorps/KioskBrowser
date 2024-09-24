import { Component, OnInit } from '@angular/core';
import {GlobalStorageService} from "../../services/global-storage.service";

@Component({
  selector: 'app-queline',
  templateUrl: './queline.component.html',
  styleUrls: ['./queline.component.css']
})
export class QuelineComponent implements OnInit {
  public nextInLineNumber: number = 0;
  public flash: boolean = false;
  constructor(public globalStorage: GlobalStorageService) { }

  public ngOnInit(): void {
    this.globalStorage.connectionReady.subscribe(() => {
      this.globalStorage.changeReceived.subscribe(() => {
        this.receiveNextInLine();
      });

      this.receiveNextInLine();
    });
  }

  private receiveNextInLine() {
    const nextInLineNumber = parseInt(this.globalStorage.get("nextInLineNumber")) ?? 1;

    if (isNaN(nextInLineNumber)) {
      this.nextInLineNumber = 1;
    } else {
      this.nextInLineNumber = nextInLineNumber;
    }
    this.flash = true;
    setTimeout(() => {
      this.flash = false;
    }, 2000);
  }

}
