import { TestBed } from '@angular/core/testing';

import { PingHubService } from './ping-hub.service';
import {TransferHubService} from "./transfer-hub.service";
import {DataHubService} from "./data-hub.service";

describe('DataHubService', () => {
  let service: DataHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DataHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
