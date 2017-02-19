import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms'

import { AppComponent } from './app.component';
import { MyComponent } from './mycomp.component';

@NgModule({
  imports: [BrowserModule, FormsModule],
  declarations: [AppComponent, MyComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
