import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { ratingRange } from '../../../directives/rating-range.directive';

import 'rxjs';
import { EmployeeService } from '../../../services/employee.service';
import { Employee } from '../../../models/employee';


@Component({
  selector: 'app-edit-employee',
  templateUrl: './edit-employee.component.html',
  styleUrls: ['./edit-employee.component.css']
})
export class EditEmployeeComponent implements OnInit {

  employeeId: Number;
  editemployeeform: FormGroup;

  constructor(private route: ActivatedRoute, private fb: FormBuilder, private employeeService: EmployeeService) {
    this.employeeId = this.route.snapshot.params['id'];
  }


  ngOnInit() {
    console.log('ngOnInit called.');
    console.log(`employeeId is ${this.employeeId}`);
    this.editemployeeform = this.fb.group({
      EmployeeId: this.employeeId,
      FirstName: ['', Validators.required],
      LastName: '',
      Age: '',
      Rating: ['', ratingRange(2, 6)]
    });
    this.employeeService.getEmployee(this.employeeId).subscribe(
      x => {
        this.editemployeeform.patchValue({
          EmployeeId: x.EmployeeId,
          FirstName: x.FirstName,
          LastName: x.LastName,
          Age: x.Age,
          //Rating: x.Rating
        });
      },
      y => console.log("Error occured : " + y)
    );
  }

  save() {
    console.log(this.editemployeeform.value);
    var result = this.employeeService.putEmployees(this.editemployeeform.value).subscribe(
      x => {
        // this.createemployeeform.patchValue({
        //   EmployeeId: x.EmployeeId
        // });
        alert('Saved Successfully.');
      },
      y => console.log("Post error occured : " + y)
    );


  }

}
