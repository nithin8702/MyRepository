//import { Directive, forwardRef, Attribute } from '@angular/core';
//import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';

import { AbstractControl } from '@angular/forms';

export function ratingRange(min: Number, max: Number) {
  return (c: AbstractControl): { [key: string]: boolean } | null => {
    //console.log(`min is ${min} and max is ${max}`);
    if (c.value !== undefined && (c.value < min || c.value > max)) {
      return { 'range': true };
    }
    return null;
  };
}