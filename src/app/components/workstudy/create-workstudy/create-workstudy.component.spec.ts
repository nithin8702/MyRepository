import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateWorkstudyComponent } from './create-workstudy.component';

describe('CreateWorkstudyComponent', () => {
  let component: CreateWorkstudyComponent;
  let fixture: ComponentFixture<CreateWorkstudyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateWorkstudyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateWorkstudyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
