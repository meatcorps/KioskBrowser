import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MessagesFrameComponent } from './messages-frame.component';

describe('MessagesFrameComponent', () => {
  let component: MessagesFrameComponent;
  let fixture: ComponentFixture<MessagesFrameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MessagesFrameComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MessagesFrameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
