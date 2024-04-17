import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SendPhotosComponent } from './send-photos.component';

describe('SendPhotosComponent', () => {
  let component: SendPhotosComponent;
  let fixture: ComponentFixture<SendPhotosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SendPhotosComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SendPhotosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
