import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

import 'rxjs';
import { EmployeeService } from '../../../services/employee.service';
import { Employee } from '../../../models/employee';


@Component({
  selector: 'app-list-employee',
  templateUrl: './list-employee.component.html',
  styleUrls: ['./list-employee.component.css']
})
export class ListEmployeeComponent implements OnInit {

  constructor(private employeeService: EmployeeService) { }

  employees: Array<Employee> = null;

  public filterText: string;
  public filterPlaceholder: string;
  public filterInput = new FormControl();


  ngOnInit() {
    this.filterText = "";
    this.filterPlaceholder = "Filter..";
    this.GetEmployees();
    this.filterInput
      .valueChanges
      .debounceTime(200)
      .subscribe(term => {
        this.filterText = term;
        console.log(term);
      });
  }

  GetEmployees(): void {
    this.employeeService.getEmployees().subscribe(
      x => {
        this.employees = x
      },
      y => console.log("Error occured : " + y)
    );
  }

  Delete(employeeId) {
    console.log('value is ' + employeeId);
    this.employeeService.deleteEmployee(employeeId).subscribe(
      x => {
        console.log('delete service response : ' + JSON.stringify(x));
        alert('deleted successfully.');
        this.GetEmployees();
      },
      y => console.log("Error occured : " + y)
    );
  }

}
