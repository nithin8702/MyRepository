import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { MaterialModule } from '@angular/material';
import 'hammerjs';
import { RouterModule } from '@angular/router'
import { CreateEmployeeComponent } from './components/employee/create.component';
import { EditEmployeeComponent } from './components/employee/edit.component';
import { ListEmployeeComponent } from './components/employee/list.component';

import { routing } from './app.routing';

@NgModule({
  imports: [BrowserModule, MaterialModule.forRoot(), routing],
  declarations: [AppComponent, CreateEmployeeComponent, EditEmployeeComponent, ListEmployeeComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
