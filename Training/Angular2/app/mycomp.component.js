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
var MyComponent = (function () {
    function MyComponent() {
        this.id = 1;
        this.name = 'Nithin';
        this.amount = 100.45678;
    }
    MyComponent = __decorate([
        core_1.Component({
            selector: 'my-comp',
            // template: `
            // <h3>Hello {{id}}</h3><br/>
            // <h3>Hello {{name}}</h3>
            // `,
            templateUrl: './app/mycomp.html',
            styleUrls: ['./app/mycomp.css']
        }), 
        __metadata('design:paramtypes', [])
    ], MyComponent);
    return MyComponent;
}());
exports.MyComponent = MyComponent;
//# sourceMappingURL=mycomp.component.js.map