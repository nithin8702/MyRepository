import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { BaseComponent } from "../../base.component";
import { WorkStudy } from '../../../models/work-study';
import { WorkstudyService } from "../../../services/workstudy.service";
import { SelectListItem } from '../../../models/selectlistitem';

import 'rxjs';

@Component({
  selector: 'app-create-workstudy',
  templateUrl: './create-workstudy.component.html',
  styleUrls: ['./create-workstudy.component.css']
})
export class CreateWorkstudyComponent extends BaseComponent implements OnInit {

  createworkstudyform: FormGroup;

  studyTypes: Array<SelectListItem> = [];  

  constructor(private fb: FormBuilder, private workstudyService: WorkstudyService) { super(); }

  ngOnInit() {

    this.workstudyService.getWorkStudy().subscribe(
      x => {
        this.studyTypes = x.StudyTypes
      },
      y => console.log("Error occured : " + y)
    );

    // this.studyTypes = [
    //   { id: "1", text: "Study Type 1" },
    //   { id: "2", text: "Study Type 2" },
    //   { id: "3", text: "Study Type 3" },
    // ];


    this.createworkstudyform = this.fb.group({
      WorkStudyID: '',
      StudyTitle: '',
      StudyDescription: '',
      StudyType: '',
      Location: '',
      StudyStatus: '',
      PlannedOutsideCost: '',
      ActualOutsideCost: '',
      StartedDate: '',
      DueDate: '',
      CompletedDate: ''
    });
  }

  save() {
    console.log(this.createworkstudyform.value);
  }

}
