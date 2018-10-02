import { TestBed } from '@angular/core/testing';

import { BaseRestApiService } from './base-rest-api.service';

describe('BaseRestApiService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: BaseRestApiService = TestBed.get(BaseRestApiService);
    expect(service).toBeTruthy();
  });
});
