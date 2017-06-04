import { Component, OnInit, ElementRef } from '@angular/core';
import { AppMenu } from './models/app-menu'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  isLogin: boolean = true;
  constructor(private elementRef: ElementRef) { }

  wsMenus: Array<AppMenu>;
  tdMenus: Array<AppMenu>;
  reportMenus: Array<AppMenu>;
  helpMenus: Array<AppMenu>;

  ngAfterViewInit() {
    // var s = document.createElement("script");
    // s.type = "text/javascript";
    // s.src = "assets/js/menus.js";
    // this.elementRef.nativeElement.appendChild(s);
  }

  ngOnInit() {
    this.wsMenus = [
      { routerLink: 'listworkstudy', href: "", icon: "fa fa-briefcase fa-stack-1x", text: "Create Work Study" },
      { routerLink: 'assignmaterial', href: "", icon: "fa fa-book fa-stack-1x", text: "Assign Material to Study" },
      { routerLink: 'workstudy1', href: "~/admin/DepartmentList", icon: "fa fa-sitemap fa-stack-1x", text: "Processing to Material" },
      { routerLink: 'workstudy1', href: "~/admin/JobTitleList", icon: "fa fa-flag fa-stack-1x", text: "Testing to Material As Is" },
    ];
    this.tdMenus = [
      { routerLink: 'workstudy2', href: "~/admin/BusinessUnitList", icon: "fa fa-briefcase fa-stack-1x", text: "Automated Process" },
      { routerLink: 'workstudy2', href: "~/admin/CourseList", icon: "fa fa-book fa-stack-1x", text: "Hardness" },
      { routerLink: 'workstudy2', href: "~/admin/DepartmentList", icon: "fa fa-sitemap fa-stack-1x", text: "Exco" },
      { routerLink: 'workstudy2', href: "~/admin/JobTitleList", icon: "fa fa-flag fa-stack-1x", text: "Fatigue Crack Growth" },
      { routerLink: 'workstudy2', href: "~/admin/DepartmentList", icon: "fa fa-sitemap fa-stack-1x", text: "DCB-Corrosion" },
      { routerLink: 'workstudy2', href: "~/admin/DepartmentList", icon: "fa fa-sitemap fa-stack-1x", text: "IGC" },
      { routerLink: 'workstudy2', href: "~/admin/DepartmentList", icon: "fa fa-sitemap fa-stack-1x", text: "Optical Mount" },
      { routerLink: 'workstudy2', href: "~/admin/DepartmentList", icon: "fa fa-sitemap fa-stack-1x", text: "Macro Etch" },
      { routerLink: 'workstudy2', href: "~/admin/DepartmentList", icon: "fa fa-sitemap fa-stack-1x", text: "SCC" }
    ];
    this.reportMenus = [
      { routerLink: 'workstudy3', href: "~/admin/BusinessUnitList", icon: "fa fa-briefcase fa-stack-1x", text: "R&D Reports" }
    ];
    this.helpMenus = [
      { routerLink: 'workstudy4', href: "~/admin/BusinessUnitList", icon: "fa fa-briefcase fa-stack-1x", text: "Help" }
    ];
  }


}
