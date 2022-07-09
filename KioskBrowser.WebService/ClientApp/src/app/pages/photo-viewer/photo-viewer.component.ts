import { Component, OnInit } from '@angular/core';
import {DataHubService} from "../../hubs/data-hub.service";
import {DomSanitizer} from "@angular/platform-browser";

@Component({
  selector: 'app-photo-viewer',
  templateUrl: './photo-viewer.component.html',
  styleUrls: ['./photo-viewer.component.css']
})
export class PhotoViewerComponent implements OnInit {
  public allPhotos: string[] = [];
  public prioPhotos: string[] = [];
  public allPhotosDone: string[] = [];
  public photoSwitch = 0;
  public photo1 = "";
  public photo2 = "";
  public addToPrio = false;

  constructor(private data: DataHubService, private sanitizer: DomSanitizer) { }

  public ngOnInit(): void {
    this.data.connectionReady.subscribe(() => {
      this.data.startListenForPhotos().subscribe(x => {
        console.log(x);
        if (this.addToPrio) {
          this.prioPhotos.push(x);
        } else {
          this.allPhotos.push(x);
        }
      });
    });

    setTimeout(() => this.addToPrio = true, 5000);

    setInterval(() => this.nextPhoto(), 10000);
  }

  public sanitize(url:string) {
    return this.sanitizer.bypassSecurityTrustUrl(url);
  }

  private nextPhoto() {
    if (this.allPhotos.length === 0) {
      this.allPhotosDone.forEach(x => this.allPhotos.push(x));
      this.allPhotosDone = [];
    }
    console.log(this.prioPhotos, this.allPhotos);
    let selectedPhoto = "";
    if (this.prioPhotos.length > 0) {
      selectedPhoto = this.prioPhotos[0];
      this.prioPhotos.splice(0, 1);
    }
    if (this.allPhotos.length === 1 && selectedPhoto === "") {
      selectedPhoto = this.allPhotos[0];
      this.allPhotos = [];
    }
    if (selectedPhoto === "") {
      const index = Math.floor(Math.random() * this.allPhotos.length);
      selectedPhoto = this.allPhotos[index];
      this.allPhotos.splice(index, 1);
    }

    this.allPhotosDone.push(selectedPhoto);

    if (this.photoSwitch === 1) {
      this.photo1 = selectedPhoto;
    } else {
      this.photo2 = selectedPhoto;
    }
    setTimeout(() => this.photoSwitch = (this.photoSwitch === 0 ? 1 : 0), 500);
    console.log(this.photo1, this.photo2);
  }

}
