import { TestBed } from '@angular/core/testing';

import { PingHubService } from './ping-hub.service';
import {TransferHubService} from "./transfer-hub.service";

describe('TransferHubService', () => {
  let service: TransferHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TransferHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
