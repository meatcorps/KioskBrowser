import { Component, OnInit } from '@angular/core';
import {Subject} from "rxjs";
import {IDartPlayerInfo} from "../../interfaces/IDartPlayerInfo";
import {GlobalStorageService} from "../../services/global-storage.service";
import {DataHubService} from "../../hubs/data-hub.service";


@Component({
  selector: 'app-dart-score',
  templateUrl: './dart-score.component.html',
  styleUrls: ['./dart-score.component.css']
})
export class DartScoreComponent implements OnInit {
  public resetSubject: Subject<void> = new Subject();
  public score1: number = -1;
  public score2: number = -1;
  public score3: number = -1;
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
        console.log('incoming', x, this.globalStorage.get(x), this.globalStorage.get(x).length);
        switch (x) {
          case "dartMode":
            const value: any = this.globalStorage.get('dartMode');
            this.mode = value;
            break;
          case "dartCurrentPlayer":
            this.currentPlayer = parseInt(this.globalStorage.get('dartCurrentPlayer'));
            this.resetSubject.next();
            break;
          case "dartPlayers":
            this.players = JSON.parse(this.globalStorage.get('dartPlayers'));
            break;
        }
      });
    });
    document.title = 'Dart Score';
  }

  public isInvalidPlayerSet(): boolean {
    const checkItems: string[] = [];

    this.players.forEach(x => {
      if (x.name.trim() !== '' && checkItems.findIndex(cx => cx === x.name.toLowerCase().trim()) === -1) {
        checkItems.push(x.name.toLowerCase().trim())
      }
    });

    return this.players.length < 1 || checkItems.length !== this.players.length;
  }

  public startMatch() {
    this.mode = 'playerSession';
    this.globalStorage.set('dartMode', this.mode);
    this.globalStorage.set('dartPlayers', JSON.stringify(this.players));
  }

  public resetMatch() {
    if (!confirm('Are you sure?')) return;

    this.currentPlayer = 0;
    this.globalStorage.set('dartCurrentPlayer', '0');
    this.mode = 'creatingPlayers';
    this.globalStorage.set('dartMode', this.mode);
    this.players.forEach(x => { x.score = 501; } );
    this.globalStorage.set('dartPlayers', JSON.stringify(this.players));
  }

  public submitScore() {
    if (this.score1 + this.score2 + this.score3 > this.players[this.currentPlayer].score) return;

    this.players[this.currentPlayer].score -= this.score1 + this.score2 + this.score3;

    this.globalStorage.set('dartPlayers', JSON.stringify(this.players));

    if (this.players[this.currentPlayer].score === 0) {
      this.win();
      return;
    }

    this.resetSubject.next();
    if (this.currentPlayer === this.players.length - 1) {
      this.currentPlayer = 0;
    } else {
      this.currentPlayer++;
    }
    this.globalStorage.set('dartCurrentPlayer', this.currentPlayer.toString());
  }

  public win(): void {
    this.mode = 'winSession';
    this.globalStorage.set('dartMode', this.mode);
  }

  public currentTotalScore(): number {
    if (this.score1 < 0 || this.score2 < 0 || this.score3 < 0) return 0;
    return this.score1 + this.score2 + this.score3;
  }
}
