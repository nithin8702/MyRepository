import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListWorkstudyComponent } from './list-workstudy.component';

describe('ListWorkstudyComponent', () => {
  let component: ListWorkstudyComponent;
  let fixture: ComponentFixture<ListWorkstudyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListWorkstudyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListWorkstudyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
