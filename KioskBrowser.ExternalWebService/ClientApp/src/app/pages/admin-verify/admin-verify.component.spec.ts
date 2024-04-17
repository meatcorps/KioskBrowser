import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminVerifyComponent } from './admin-verify.component';

describe('AdminVerifyComponent', () => {
  let component: AdminVerifyComponent;
  let fixture: ComponentFixture<AdminVerifyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminVerifyComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AdminVerifyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
