import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MessagePictureFrameComponent } from './message-picture-frame.component';

describe('MessagePictureFrameComponent', () => {
  let component: MessagePictureFrameComponent;
  let fixture: ComponentFixture<MessagePictureFrameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MessagePictureFrameComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MessagePictureFrameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
