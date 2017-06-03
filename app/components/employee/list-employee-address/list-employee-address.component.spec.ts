import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListEmployeeAddressComponent } from './list-employee-address.component';

describe('ListEmployeeAddressComponent', () => {
  let component: ListEmployeeAddressComponent;
  let fixture: ComponentFixture<ListEmployeeAddressComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListEmployeeAddressComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListEmployeeAddressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
