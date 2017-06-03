import { Component, OnInit } from '@angular/core';
import { EmployeeService } from '../../../services/employee.service';
import { Employee, Employee_VM } from '../../../models/employee';

@Component({
  selector: 'app-parent',
  templateUrl: './parent.component.html',
  styleUrls: ['./parent.component.css']
})
export class ParentComponent implements OnInit {

  emp: Employee_VM = new Employee_VM();
  name: string = "Nithin Rajan";
  constructor(private employeeService: EmployeeService) {
    console.log('const called');
    this.employeeService.getEmployee(11).subscribe(
      x => {
        this.emp = {
          EmployeeId: x.EmployeeId,
          FirstName: x.FirstName,
          LastName: x.LastName,
          Age: x.Age
        }
        console.log('this.emp :' + this.emp);
      },
      y => {
        console.log("Error occured : " + y)
        alert('No data found');
      }
    );
  }

  ngOnInit() {
    console.log('init called');
  }

  onVoted(agreed: boolean) {
    alert('agreed is ' + agreed);
  }

}
