import { BrowserModule, HAMMER_GESTURE_CONFIG, HammerGestureConfig, HammerModule } from '@angular/platform-browser';
import { NgModule, isDevMode } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import * as hammer from 'hammerjs';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import {NgCircleProgressModule} from "ng-circle-progress";
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {MaterialModule} from "./material/material.module";
import {SendMessageComponent} from "./pages/send-message/send-message.component";
import {SendPhotosComponent} from "./pages/send-photos/send-photos.component";
import {AdminVerifyComponent} from "./pages/admin-verify/admin-verify.component";
import { ServiceWorkerModule } from '@angular/service-worker';

export class MyHammerConfig extends HammerGestureConfig {
  overrides = <any>{
    swipe: { direction: hammer.DIRECTION_HORIZONTAL },
    pinch: { enable: false },
    rotate: { enable: false }
  }};

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AdminVerifyComponent,
    SendMessageComponent,
    SendPhotosComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HammerModule,
    HttpClientModule,
    FormsModule,
    MaterialModule,
    NgCircleProgressModule.forRoot({
      // set defaults here
      radius: 100,
      outerStrokeWidth: 16,
      innerStrokeWidth: 8,
      outerStrokeColor: "#78C000",
      innerStrokeColor: "#C7E596",
      animationDuration: 300,
    }),
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'home', component: HomeComponent },
      { path: 'send-message', component: SendMessageComponent },
      { path: 'send-photo', component: SendPhotosComponent },
      { path: 'admin-verify', component: AdminVerifyComponent },
    ]),
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: !isDevMode(),
      // Register the ServiceWorker as soon as the application is stable
      // or after 30 seconds (whichever comes first).
      registrationStrategy: 'registerWhenStable:30000'
    }),
  ],
  providers: [
    provideAnimationsAsync(),
    { provide: HAMMER_GESTURE_CONFIG, useClass: MyHammerConfig }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
