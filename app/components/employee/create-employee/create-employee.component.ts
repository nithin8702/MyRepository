import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { ratingRange } from '../../../directives/rating-range.directive';

import 'rxjs';
import { EmployeeService } from '../../../services/employee.service';
import { Employee } from '../../../models/employee';

@Component({
  selector: 'app-create-employee',
  templateUrl: './create-employee.component.html',
  styleUrls: ['./create-employee.component.css']
})
export class CreateEmployeeComponent implements OnInit {

  createemployeeform: FormGroup;

  constructor(private fb: FormBuilder, private employeeService: EmployeeService) { }

  employees: Array<Employee> = null;

  ngOnInit() {
    this.createemployeeform = this.fb.group({
      EmployeeId: 0,
      FirstName: ['', Validators.required],
      LastName: '',
      Age: '',
      Rating: ['', ratingRange(2, 6)]
    });
    this.createemployeeform.get('Rating').valueChanges.subscribe(x => console.log(x));
  }

  save() {
    var result = this.employeeService.postEmployees(this.createemployeeform.value).subscribe(
      x => {
        this.createemployeeform.patchValue({
          EmployeeId: x.EmployeeId
        });
        alert('Saved Successfully.');
      },
      y => console.log("Post error occured : " + y)
    );
  }

}
