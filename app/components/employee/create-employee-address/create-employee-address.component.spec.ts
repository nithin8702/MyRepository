import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEmployeeAddressComponent } from './create-employee-address.component';

describe('CreateEmployeeAddressComponent', () => {
  let component: CreateEmployeeAddressComponent;
  let fixture: ComponentFixture<CreateEmployeeAddressComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEmployeeAddressComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEmployeeAddressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
