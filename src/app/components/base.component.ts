import { Component, OnInit } from '@angular/core';
import { IMyOptions, IMyDate, IMyDateModel } from 'mydatepicker';

@Component({
    selector: 'app-base',
    template: '',
})
export abstract class BaseComponent implements OnInit {

    constructor() { }

    ngOnInit() {
    }

    myOptions: IMyOptions = {
        dateFormat: 'mm/dd/yyyy',
        width: '57%'
    }

}
