import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Subject} from "rxjs";

@Component({
  selector: 'app-dart-arrow',
  templateUrl: './dart-arrow.component.html',
  styleUrls: ['./dart-arrow.component.css']
})
export class DartArrowComponent implements OnInit {
  public scoreItems: number[] = [0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,25,50 ];
  public score: number = -1;
  public multiply: number = 1;

  @Input() reset: Subject<void>;

  @Output() change = new EventEmitter<number>();

  constructor() { }

  public ngOnInit(): void {
    if (this.reset) {
      this.reset.subscribe(() => {
        this.score = -1;
        this.multiply = 1;
        this.update();
      });
    }
  }

  public checkMultiply($event: any) {
    if ($event === 25 || $event === 50) {
      this.multiply = 1;
    }
    this.score = parseInt($event);
    this.update();
  }

  public setMultiply($event: any) {
    this.multiply = parseInt($event);
    this.update();
  }

  public update() {
    if (this.change) {
      this.change.next(this.total);
    }
  }

  public get total(): number {
    return this.score * this.multiply;
  }
}
