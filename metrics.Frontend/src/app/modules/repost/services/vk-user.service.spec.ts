import { TestBed } from '@angular/core/testing';

import { VkUserService } from './vk-user.service';

describe('VkUserServiceService', () => {
  let service: VkUserService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VkUserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
