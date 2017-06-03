import { Injectable } from '@angular/core';
import { Http, Headers, Response, ResponseOptions } from '@angular/http';
import { Employee } from '../models/employee';
import { Employeeaddress } from '../models/employeeaddress';
import { Observable } from 'rxjs/Observable';
import 'rxjs';

@Injectable()
export class EmployeeService {

  private baseUrl = 'http://localhost/staticdata/api/employees';

  constructor(private http: Http) {
    //this.baseUrl = 'http://localhost:52892/api/employees';
  }

  getEmployees(): Observable<Employee[]> {
    return this.http.get(this.baseUrl).map(x => x.json());
  }

  getEmployee(id: Number): Observable<Employee> {
    return this.http.get(this.baseUrl + '/' + id).map(x => x.json());
  }

  postEmployees(employee: any): any {
    let headers = new Headers({ 'Content-Type': 'application/json' });
    return this.http.post(this.baseUrl, JSON.stringify(employee), { headers: headers }).map(res => res.json());
  }

  putEmployees(employee: any): any {
    let headers = new Headers({ 'Content-Type': 'application/json' });
    return this.http.put(this.baseUrl + '/' + employee.EmployeeId, JSON.stringify(employee), { headers: headers }).map(res => res.json());
  }

  deleteEmployee(id: Number): any {
    let headers = new Headers({ 'Content-Type': 'application/json' });
    return this.http.delete(this.baseUrl + '/' + id, { headers: headers }).map(res => res.json());
  }

}
