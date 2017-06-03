import { Directive, ElementRef, Input } from '@angular/core';

@Directive({
  selector: '[appMyhighlight]'
})
export class MyhighlightDirective {

  constructor(el: ElementRef) {
    el.nativeElement.style.backgroundColor = 'yellow';
  }

}
