import {Component, OnInit} from '@angular/core';
import {DataHubService} from "../../hubs/data-hub.service";
import {Router} from "@angular/router";
import {CodeService} from "../../services/code.service";
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-send-message',
  templateUrl: './send-message.component.html',
  styleUrl: './send-message.component.css'
})
export class SendMessageComponent implements OnInit {
  public who: string = "";
  public message: string = "";
  public sendingMessage: boolean = false;

  constructor(private dataHub: DataHubService, private code: CodeService, private router: Router, private snackBar: MatSnackBar) {
  }

  public ngOnInit(): void {
      if (localStorage.getItem("who") !== null)
        this.who = localStorage.getItem("who")!;
  }

  public async send(who: string, message: string) {
    localStorage.setItem("who", who);
    this.sendingMessage = true;
    await this.dataHub.sendMessage(this.code.currentCode(), who.trim().replace('|', ''), message.trim().replace('|', ''));

    this.snackBar.open("Message send! Will be redirected in 5 seconds", '', {
      duration: 5000
    });

    setTimeout(() => {
      this.sendingMessage = false;
      this.message = "";
      this.router.navigateByUrl('/home');
    }, 5000);
  }
}
