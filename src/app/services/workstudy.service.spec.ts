import { TestBed, inject } from '@angular/core/testing';

import { WorkstudyService } from './workstudy.service';

describe('WorkstudyService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WorkstudyService]
    });
  });

  it('should be created', inject([WorkstudyService], (service: WorkstudyService) => {
    expect(service).toBeTruthy();
  }));
});
