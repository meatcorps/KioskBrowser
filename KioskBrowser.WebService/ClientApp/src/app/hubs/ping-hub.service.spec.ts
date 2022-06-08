import { TestBed } from '@angular/core/testing';

import { PingHubService } from './ping-hub.service';

describe('PingHubService', () => {
  let service: PingHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PingHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
