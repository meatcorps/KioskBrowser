import {Component, OnDestroy, OnInit} from '@angular/core';
import {INextToVerifyObject, IVerifyObject, TransferHubService} from "../../hubs/transfer-hub.service";
import {DataHubService} from "../../hubs/data-hub.service";
import {CodeService} from "../../services/code.service";
import {filter, interval, Observable, Subscription} from "rxjs";
import {SwPush} from "@angular/service-worker";

@Component({
  selector: 'app-admin-verify',
  templateUrl: './admin-verify.component.html',
  styleUrl: './admin-verify.component.css'
})
export class AdminVerifyComponent implements OnInit, OnDestroy {
  public admin: string = "";
  public ready: boolean = false;
  public userCodeInput: string = "";
  public mode: string = "pho";
  public test: string = "";
  // @ts-ignore
  public current: INextToVerifyObject;
  public animationEffectAccept: number = 0;
  public animationEffectDecline: number = 0;
  private loading: boolean = false;
  public totalMessages: number = 0;
  public totalPictures: number = 0;
  private subscription: Subscription = new Subscription();

  constructor(
    private transferHub: TransferHubService,
    private dataHub: DataHubService,
    private code: CodeService,
    private swPush: SwPush) {
  }

  public async switch(to: string): Promise<void> {
    this.ready = false;
    this.mode = to;
    await this.loadNext();
    this.ready = true;
  }

  public async loadNext(): Promise<void> {
    try {
      this.current = await this.transferHub.getNextVerifyObject(this.code.currentCode(), this.admin, this.mode);
    } catch (e) {
      this.current = {total: 0, object: {Extension: "", Data: "", Id: "", Who: "", Message: "", Type: ""}};
    }
  }

  public getMime(obj: IVerifyObject): string {
    return obj.Extension == 'gif' ? 'image/gif' : 'image/jpeg';
  }

  public async ngOnInit(): Promise<void> {
    const result = localStorage.getItem('adminCode');
    this.ready = false;
    if (result === null)
      return;

    await this.enterCode(result!);

    setInterval(() => {
      this.animationEffectAccept -= 0.01;
      this.animationEffectDecline -= 0.01;
      if (this.animationEffectAccept < 0)
        this.animationEffectAccept = 0;
      if (this.animationEffectDecline < 0)
        this.animationEffectDecline = 0;
    }, 16);

    if (this.admin)
      await this.loadNext();

    this.ready = true;
    this.subscription = interval(2000)
      .pipe(filter(x => this.dataHub.connected))
      .subscribe(async () => {
        this.totalMessages = await this.dataHub.totalToVerifyMessage(this.code.currentCode());
        this.totalPictures = await this.dataHub.totalToVerifyPicture(this.code.currentCode());
      });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public async enterCode(userCodeInput: string): Promise<void> {
    if (await this.dataHub.adminCode(userCodeInput)) {
      this.admin = userCodeInput;
      localStorage.setItem('adminCode', userCodeInput);
      return;
    }

    this.admin = '';
  }

  public async accept() {
    if (this.loading || this.current.total === 0)
      return;

    this.loading = true;
    this.animationEffectAccept = 1;
    await this.transferHub.getVerificationForObject(this.code.currentCode(), this.admin, this.mode, true);
    await this.loadNext();
    this.loading = false;
  }

  public async decline() {
    if (this.loading || this.current.total === 0)
      return;

    this.loading = true;
    this.animationEffectDecline = 1;
    await this.transferHub.getVerificationForObject(this.code.currentCode(), this.admin, this.mode, false);
    await this.loadNext();
    this.loading = false;
  }

  public async enablePushNotifications() {
    console.log("enablePushNotifications", await this.dataHub.pushPublicKey());
    this.swPush.requestSubscription({
      serverPublicKey: await this.dataHub.pushPublicKey()
    })
      .then(sub => {
        console.log(sub.toJSON());
        this.dataHub.addPushSubscription(sub);
      })
      .catch(err => console.error("Could not subscribe to notifications", err));
  }
}
