import { Component } from '@angular/core';

@Component({
    selector: 'my-comp',
    // template: `
    // <h3>Hello {{id}}</h3><br/>
    // <h3>Hello {{name}}</h3>
    // `,
    templateUrl: './app/mycomp.html',
    styleUrls: ['./app/mycomp.css']
})
export class MyComponent {
    id: Number = 1;
    name = 'Nithin';
    amount = 100.45678;
}
