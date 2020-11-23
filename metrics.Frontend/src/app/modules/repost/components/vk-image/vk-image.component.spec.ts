import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { VkImageComponent } from './vk-image.component';

describe('VkImageComponent', () => {
  let component: VkImageComponent;
  let fixture: ComponentFixture<VkImageComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ VkImageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VkImageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
