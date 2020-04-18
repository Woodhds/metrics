import { TestBed } from '@angular/core/testing';

import { VkMessageService } from './vk-message.service';

describe('VkMessageService', () => {
  let service: VkMessageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VkMessageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
