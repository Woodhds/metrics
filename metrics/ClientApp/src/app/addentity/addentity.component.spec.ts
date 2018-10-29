import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddentityComponent } from './addentity.component';

describe('AddentityComponent', () => {
  let component: AddentityComponent;
  let fixture: ComponentFixture<AddentityComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddentityComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddentityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
