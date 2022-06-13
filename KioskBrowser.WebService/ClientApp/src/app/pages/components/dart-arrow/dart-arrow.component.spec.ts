import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DartArrowComponent } from './dart-arrow.component';

describe('DartArrowComponent', () => {
  let component: DartArrowComponent;
  let fixture: ComponentFixture<DartArrowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DartArrowComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DartArrowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
