import { TestBed } from '@angular/core/testing';

import { VkUserServiceService } from './vk-user-service.service';

describe('VkUserServiceService', () => {
  let service: VkUserServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VkUserServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
