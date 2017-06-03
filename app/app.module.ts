import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { AppComponent } from './app.component';

import { CreateEmployeeComponent } from './components/employee/create-employee/create-employee.component';
import { EditEmployeeComponent } from './components/employee/edit-employee/edit-employee.component';
import { ListEmployeeComponent } from './components/employee/list-employee/list-employee.component';
import { CreateEmployeeAddressComponent } from './components/employee/create-employee-address/create-employee-address.component';
import { EditEmployeeAddressComponent } from './components/employee/edit-employee-address/edit-employee-address.component';
import { ListEmployeeAddressComponent } from './components/employee/list-employee-address/list-employee-address.component';

import { EmployeeService } from './services/employee.service';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { routing } from './app.routing';
import { NavbarComponent } from './components/navbar/navbar.component';
import { HomeComponent } from './components/home/home.component';
import { CapitializePipe } from './pipes/capitialize.pipe';
import { MyhighlightDirective } from './directives/myhighlight.directive';
import { ParentComponent } from './components/employee/parent/parent.component';
import { ChildComponent } from './components/employee/child/child.component';

import { EmployeePipe } from './pipes/employee.pipe';

@NgModule({
  declarations: [
    AppComponent,
    CreateEmployeeComponent,
    EditEmployeeComponent,
    ListEmployeeComponent,
    CreateEmployeeAddressComponent,
    EditEmployeeAddressComponent,
    ListEmployeeAddressComponent,
    NavbarComponent,
    HomeComponent,
    CapitializePipe,
    MyhighlightDirective,
    ParentComponent,
    ChildComponent,
    EmployeePipe
    //RatingRangeDirective,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    routing,
    NgbModule.forRoot()
  ],
  providers: [EmployeeService],
  bootstrap: [AppComponent]
})
export class AppModule { }
