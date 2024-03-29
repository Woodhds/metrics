import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { RepostComponent } from './repost.component';

describe('RepostComponent', () => {
  let component: RepostComponent;
  let fixture: ComponentFixture<RepostComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ RepostComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RepostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
