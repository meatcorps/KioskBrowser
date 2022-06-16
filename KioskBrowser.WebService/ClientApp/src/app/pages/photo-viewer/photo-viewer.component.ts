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
  public allPhotosDone: string[] = [];
  public photoSwitch = 0;
  public photo1 = "";
  public photo2 = "";

  constructor(private data: DataHubService, private sanitizer: DomSanitizer) { }

  public ngOnInit(): void {
    this.data.connectionReady.subscribe(() => {
      this.data.startListenForPhotos().subscribe(x => {
        this.allPhotos.push(x);
      });
    });

    setInterval(() => this.nextPhoto(), 5000);
  }

  public sanitize(url:string) {
    return this.sanitizer.bypassSecurityTrustUrl(url);
  }

  private nextPhoto() {
    if (this.allPhotos.length === 0) {
      this.allPhotosDone.forEach(x => this.allPhotos.push(x));
      this.allPhotosDone = [];
    }
    let selectedPhoto = "";
    if (this.allPhotos.length === 1) {
      selectedPhoto = this.allPhotos[0];
      this.allPhotos = [];
    } else {
      const index = Math.floor(Math.random() * this.allPhotos.length);
      selectedPhoto = this.allPhotos[index];
      this.allPhotos.splice(index, 1);
    }

    this.allPhotosDone.push(selectedPhoto);
    this.photoSwitch = (this.photoSwitch === 0 ? 1 : 0);

    if (this.photoSwitch === 0) {
      this.photo1 = selectedPhoto;
    } else {
      this.photo2 = selectedPhoto;
    }
  }

}
