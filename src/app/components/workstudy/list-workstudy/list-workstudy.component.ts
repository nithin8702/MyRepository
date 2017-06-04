import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { BaseComponent } from "../../base.component";
import { WorkStudy } from '../../../models/work-study';
import { WorkstudyService } from "../../../services/workstudy.service";

import { Subject } from 'rxjs/Rx';
import 'rxjs/add/operator/map';

@Component({
  selector: 'app-list-workstudy',
  templateUrl: './list-workstudy.component.html',
  styleUrls: ['./list-workstudy.component.css']
})
export class ListWorkstudyComponent extends BaseComponent implements OnInit {

  dtOptions: DataTables.Settings = {
     //processing:true,
    //serverSide:true,
    // ajax: {
    //         "url": "http://localhost:35836/api/workstudy/1",
    //         "type": "POST",
    //         // success: function (x) {
    //         //   this.workStudies = x;
    //         //   this.dtTrigger.next();
    //         // },
    //     },
  };

  workStudies: Array<WorkStudy>;

  dtTrigger: Subject<WorkStudy> = new Subject();

  constructor(private route: Router, private workstudyService: WorkstudyService) { super() }

  ngOnInit() {
    this.workstudyService.getWorkStudies().subscribe(
      x => {
        this.workStudies = x;
        this.dtTrigger.next();
      },
      y => console.log("Error occured : " + y)
    );
  }

  btnclick() {
    this.route.navigate(['createworkstudy']);
  }

}
