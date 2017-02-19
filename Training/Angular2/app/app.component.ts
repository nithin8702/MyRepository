import { Component } from '@angular/core';

@Component({
  selector: 'my-app',
  template: `<h1>Hello {{name}}</h1>
  <input type='text' value={{msg}}/>
  <br/><hr/>
  <input [(ngModel)]='msg'/>
  <button (click)='showmsg();'>Click Here</button>
  <my-comp *ngIf ='check'></my-comp>
  <ul *ngFor='let emp of employees'>
  <li>{{emp.id + ' - ' + emp.name}}</li>
  </ul>
  `,
})
export class AppComponent {
  name = 'Angular';
  check: Boolean = true;
  msg: string = "Hello World!";
  employees = [{ id: 1, name: "nithin 1" }, { id: 2, name: "nithin 2" }];
  showmsg() {
    console.log(this.msg);
  }
}
