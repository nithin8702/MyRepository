import { Injectable } from '@angular/core';
import { Http, Headers, Response, ResponseOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs';
import { WorkStudy } from '../models/work-study';


@Injectable()
export class WorkstudyService {
  private baseUrl = 'http://localhost:35836/api/workstudy';
  constructor(private http: Http) { }

  getWorkStudies(): Observable<WorkStudy[]> {
    this.baseUrl = 'http://localhost:35836/api/workstudy/1';
    let headers = new Headers({
      'Content-Type': 'application/json',
      "Token": "4be0005b-e8da-42d0-96b0-9f9acd059a270a5148fe-04b7-4519-a242-a796e6d08666"
    });
    return this.http.get(this.baseUrl).map(x => x.json());
  }

  getWorkStudy(): Observable<WorkStudy> {
    let headers = new Headers({
      'Content-Type': 'application/json',
      "Token": "4be0005b-e8da-42d0-96b0-9f9acd059a270a5148fe-04b7-4519-a242-a796e6d08666"
    });
    return this.http.get(this.baseUrl, { headers: headers }).map(x => x.json());
  }

}
