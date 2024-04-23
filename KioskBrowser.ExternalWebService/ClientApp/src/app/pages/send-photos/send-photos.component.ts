import {
  AfterViewInit,
  Component,
  ElementRef,
  HostListener,
  ViewChild
} from '@angular/core';
import {Router} from "@angular/router";
import {TransferHubService} from "../../hubs/transfer-hub.service";
import {DataHubService} from "../../hubs/data-hub.service";
import {CodeService} from "../../services/code.service";
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-send-photos',
  templateUrl: './send-photos.component.html',
  styleUrl: './send-photos.component.css'
})
export class SendPhotosComponent implements AfterViewInit  {
  public toUpload: ToUpload[] = [];
  public uploading: boolean = false;

  public uploadPercentage: number = 0;
  public statusText: string = "Picture 1 of 10";
  public currentImage: string = "";

  @ViewChild('photoInput')
  private uploader!: ElementRef<HTMLInputElement>;
  private uploadActivated: boolean = false;

  @HostListener("window:focus", ["$event"])
  public onFocus(event: FocusEvent): void {
    setTimeout(async () => {
      if (this.uploadActivated && this.uploader.nativeElement.files!.length == 0)
        await this.router.navigateByUrl('/home');

      this.uploadActivated = false;
    }, 1000)
  }

  constructor(private router: Router, private transferHub: TransferHubService, private codeService: CodeService, private snackBar: MatSnackBar) {
  }

  public async ngAfterViewInit(): Promise<void> {
    this.uploading = false;
    this.toUpload = [];
    const isActive = navigator.userActivation?.isActive ?? false;
    this.uploadActivated = isActive;

    if (isActive) {
      this.uploader.nativeElement.click();
    } else
      await this.router.navigateByUrl('/home');
  }

  public async uploadPhotoInputChange($event: Event): Promise<void> {
    const files = this.uploader.nativeElement.files!;
    for (let i = 0; i < files.length; i++) {
      let data = await this.toBase64(files[i]);
      this.toUpload.push(new ToUpload(files[i].name, data, "", data.indexOf("video/") > -1));
    }
  }

  public async upload(): Promise<void> {
    this.uploading = true;
    for (let i = 0; i < this.toUpload.length; i++) {
      let data = this.toUpload[i].base64;
      const type = data.split(";base64")[0].replace("data:", "");
      data = data.split(",")[1];
      this.currentImage = this.toUpload[i].base64;
      this.statusText = "Picture / Video " + (i + 1) + " of " + this.toUpload.length;
      this.uploadPercentage = 0;
      const chunkSize = await this.transferHub.chunkSize();
      const id = await this.transferHub.transferRequest(this.codeService.currentCode(), this.toUpload[i].name, data.length, this.toUpload[i].metadata, type);
      const chunks = this.chunkSubString(data, chunkSize);
      console.log(chunks[5]);
      for (let j = 0; j < chunks.length; j++) {

        const nextChunk = await this.transferHub.sendData(id, j, chunks[j]);

        this.uploadPercentage = Math.floor(((j / chunks.length) * 1000) / 10);

        console.log(
          "TRANSFER IN PROCESS",
          this.toUpload[i].name,
          j,
          nextChunk,
          chunks.length,
          Math.floor(((j / chunks.length) * 1000) / 10) + "%");
      }
      this.uploadPercentage = 100;
      this.snackBar.open("Photo / video " + (i + 1) + " send", '', {
        duration: 2000
      });
    }

    this.snackBar.open("All photo(s) or video(s) are send! You will be redirected after 5 seconds.", '', {
      duration: 5000
    });

    setTimeout(async () => {

      this.uploading = false;
      await this.router.navigateByUrl('/home');
    }, 5000);
  }

  private chunkSubString(str: string, size: number): string[] {
    const numChunks = Math.ceil(str.length / size)
    const chunks = new Array(numChunks)

    for (let i = 0, o = 0; i < numChunks; ++i, o += size) {
      chunks[i] = str.substring(o, o + size);
      console.log(chunks[i].length);
    }

    return chunks
  }

  private async toBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = error => reject(error);
    });
  }
}

class ToUpload {
  constructor(public name: string, public base64: string, public metadata: string, public video: boolean) {
  }
}
