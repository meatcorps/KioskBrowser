import { Component, OnInit } from '@angular/core';
import {IDartPlayerInfo} from "../../interfaces/IDartPlayerInfo";
import {DataHubService} from "../../hubs/data-hub.service";
import {GlobalStorageService} from "../../services/global-storage.service";

@Component({
  selector: 'app-messages-frame',
  templateUrl: './messages-frame.component.html',
  styleUrls: ['./messages-frame.component.css']
})
export class MessagesFrameComponent implements OnInit {
  public mode: 'creatingPlayers'|'playerSession'|'winSession' = 'creatingPlayers';
  public players: IDartPlayerInfo[] = [];
  public currentPlayer = 0;

  constructor(private data: DataHubService, private globalStorage: GlobalStorageService) { }

  ngOnInit(): void {
    this.globalStorage.connectionReady.subscribe(() => {
      const value: any = this.globalStorage.getWithDefault('dartMode', 'creatingPlayers');
      this.mode = value;
      this.currentPlayer = parseInt(this.globalStorage.getWithDefault('dartCurrentPlayer', '0'));
      this.players = JSON.parse(this.globalStorage.getWithDefault('dartPlayers', JSON.stringify([])));
      console.log(this.mode, this.currentPlayer, this.players);
      this.globalStorage.changeReceived.subscribe(x => {
        switch (x) {
          case "dartMode":
            const value: any = this.globalStorage.get('dartMode');
            this.mode = value;
            break;
          case "dartCurrentPlayer":
            this.currentPlayer = parseInt(this.globalStorage.get('dartCurrentPlayer'));
            break;
          case "dartPlayers":
            this.players = JSON.parse(this.globalStorage.get('dartPlayers'));
            break;
        }
      });
    });
    document.title = 'Dart Score';
  }

}
