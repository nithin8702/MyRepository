import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'capitialize'
})
export class CapitializePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    if (!value) return value;
    return value.replace(/\w\S*/g, function (txt) {
      return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
    });
  }

}
