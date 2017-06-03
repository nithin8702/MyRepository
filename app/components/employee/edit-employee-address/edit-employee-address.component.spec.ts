import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditEmployeeAddressComponent } from './edit-employee-address.component';

describe('EditEmployeeAddressComponent', () => {
  let component: EditEmployeeAddressComponent;
  let fixture: ComponentFixture<EditEmployeeAddressComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditEmployeeAddressComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditEmployeeAddressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
