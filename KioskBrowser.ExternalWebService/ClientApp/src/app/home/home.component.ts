import { Component } from '@angular/core';
import {Router} from "@angular/router";
import {CodeService} from "../services/code.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']

})
export class HomeComponent {
  private counter: number = 0;

  constructor(private router: Router, public codeService: CodeService) {
  }

  public async goToAdmin(): Promise<void> {
    this.counter++;
    if (this.counter >= 7)
      await this.router.navigateByUrl('/admin-verify');
  }
}
