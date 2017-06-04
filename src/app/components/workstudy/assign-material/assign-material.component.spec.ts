import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignMaterialComponent } from './assign-material.component';

describe('AssignMaterialComponent', () => {
  let component: AssignMaterialComponent;
  let fixture: ComponentFixture<AssignMaterialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssignMaterialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignMaterialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
