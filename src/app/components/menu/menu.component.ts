import { Component, OnInit, Input } from '@angular/core';
import { AppMenu } from '../../models/app-menu'
import { Router } from '@angular/router';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {

  constructor(private route: Router) { }
  @Input() menus: Array<AppMenu>;
  ngOnInit() {
  }

  // routes(){
  //   alert('cc');
  //   this.route.navigate(['home/workstudy']);
  //   return false;
  // }

}
