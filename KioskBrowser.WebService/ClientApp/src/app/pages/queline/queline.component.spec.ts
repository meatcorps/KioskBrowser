import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuelineComponent } from './queline.component';

describe('QuelineComponent', () => {
  let component: QuelineComponent;
  let fixture: ComponentFixture<QuelineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QuelineComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QuelineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
