import { Component, OnInit } from '@angular/core';
import {IDartPlayerInfo} from "../../interfaces/IDartPlayerInfo";
import {DataHubService} from "../../hubs/data-hub.service";
import {GlobalStorageService} from "../../services/global-storage.service";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";
import {delay} from "rxjs/operators";
import {DomSanitizer, SafeResourceUrl} from "@angular/platform-browser";

@Component({
  selector: 'app-messages-frame',
  templateUrl: './messages-frame.component.html',
  styleUrls: ['./messages-frame.component.css']
})
export class MessagesFrameComponent implements OnInit {
  public code = "";
  public message: IMessagePicture;
  public picture: IMessagePicture;
  public animate: number;

  private cancelToken = 0;

  constructor(private globalStorage: GlobalStorageService, private http: HttpClient, protected _sanitizer: DomSanitizer) { }

  ngOnInit(): void {
    this.globalStorage.connectionReady.subscribe(async () => {
      const result: any = await this.http.get(environment.url + '/currentcode').toPromise();
      this.code = result;
      clearInterval(this.cancelToken);
      await this.update();
      this.cancelToken = setInterval(async () => {
        await this.update();
      }, 10000);
    });
    document.title = 'Message picture frame';
  }

  public getMime(message: IMessagePicture): SafeResourceUrl {
    return this._sanitizer.bypassSecurityTrustResourceUrl('data:image/' + (message.extension == 'gif' ? 'gif' : 'jpeg') + ';base64,' + message.data);
  }

  private async update(): Promise<void> {
    await this.animateDown();
    const message: any = await this.http.get(environment.url + '/nextmessage').toPromise();
    this.message = message;
    console.log(this.message);

    const photo: any = await this.http.get(environment.url + '/nextphoto').toPromise();
    this.picture = photo;
    await this.animateUp();
  }

  private async animateDown(): Promise<void> {
    while (this.animate > 0) {
      this.animate -= 0.01;
      await new Promise(r => setTimeout(r, 10));
    }
    this.animate = 0;
  }

  private async animateUp(): Promise<void> {
    while (this.animate < 1) {
      this.animate += 0.01;
      await new Promise(r => setTimeout(r, 10));
    }
    this.animate = 1;
  }
}

interface IMessagePicture {
  id: string,
  type: string,
  who: string,
  message: string,
  data: string,
  extension: string,
}
