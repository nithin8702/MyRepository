import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { AddEmployeeComponent } from './employee/add.component';
import { EditEmployeeComponent } from './employee/edit.component';

import { routing } from './app.routing';

@NgModule({
  imports: [BrowserModule, routing],
  declarations: [AppComponent, AddEmployeeComponent, EditEmployeeComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
