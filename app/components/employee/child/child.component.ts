import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Employee, Employee_VM } from '../../../models/employee';

@Component({
  selector: 'app-child',
  templateUrl: './child.component.html',
  styleUrls: ['./child.component.css']
})
export class ChildComponent implements OnInit {

  @Input() emp: Employee_VM;
  @Input('name') fullName: string;

  @Output() onVoted = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit() {
  }

  vote(agreed: boolean) {
    this.onVoted.emit(agreed);
  }

}
