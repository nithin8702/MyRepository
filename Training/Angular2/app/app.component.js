"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var AppComponent = (function () {
    function AppComponent() {
        this.name = 'Angular';
        this.check = true;
        this.msg = "Hello World!";
        this.employees = [{ id: 1, name: "nithin 1" }, { id: 2, name: "nithin 2" }];
    }
    AppComponent.prototype.showmsg = function () {
        console.log(this.msg);
    };
    AppComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            template: "<h1>Hello {{name}}</h1>\n  <input type='text' value={{msg}}/>\n  <br/><hr/>\n  <input [(ngModel)]='msg'/>\n  <button (click)='showmsg();'>Click Here</button>\n  <my-comp *ngIf ='check'></my-comp>\n  <ul *ngFor='let emp of employees'>\n  <li>{{emp.id + ' - ' + emp.name}}</li>\n  </ul>\n  ",
        }), 
        __metadata('design:paramtypes', [])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map