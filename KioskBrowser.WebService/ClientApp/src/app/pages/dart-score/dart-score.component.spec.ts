import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DartScoreComponent } from './dart-score.component';

describe('DartScoreComponent', () => {
  let component: DartScoreComponent;
  let fixture: ComponentFixture<DartScoreComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DartScoreComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DartScoreComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
