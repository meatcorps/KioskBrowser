import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ManagementComponent } from './pages/management/management.component';
import {MaterialModule} from "./material/material.module";
import { GroupManagerComponent } from './pages/management/group-manager/group-manager.component';
import { ProductManagerComponent } from './pages/management/product-manager/product-manager.component';
import { MenuComponent } from './pages/menu/menu.component';
import { SupplyComponent } from './pages/supply/supply.component';
import { DartScoreComponent } from './pages/dart-score/dart-score.component';
import { DartArrowComponent } from './pages/components/dart-arrow/dart-arrow.component';
import { MessagesFrameComponent } from './pages/messages-frame/messages-frame.component';
import { PhotoViewerComponent } from './pages/photo-viewer/photo-viewer.component';
import { QuelineComponent } from './pages/queline/queline.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    ManagementComponent,
    GroupManagerComponent,
    ProductManagerComponent,
    MenuComponent,
    SupplyComponent,
    DartScoreComponent,
    DartArrowComponent,
    MessagesFrameComponent,
    PhotoViewerComponent,
    QuelineComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    MaterialModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'management', component: ManagementComponent },
      { path: 'menu', component: MenuComponent },
      { path: 'supply', component: SupplyComponent },
      { path: 'darts', component: DartScoreComponent },
      { path: 'photo', component: PhotoViewerComponent },
      { path: 'messageframe', component: MessagesFrameComponent },
      { path: 'queline', component: QuelineComponent },
    ]),
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
