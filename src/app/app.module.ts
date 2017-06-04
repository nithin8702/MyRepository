import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Angular2FontAwesomeModule } from 'angular2-font-awesome/angular2-font-awesome';

import { DataTablesModule } from 'angular-datatables';

import { AppComponent } from './app.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { MenuComponent } from './components/menu/menu.component';
import { routing } from './app.routing';
import { CreateWorkstudyComponent } from './components/workstudy/create-workstudy/create-workstudy.component';
import { AssignMaterialComponent } from './components/workstudy/assign-material/assign-material.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { ListWorkstudyComponent } from './components/workstudy/list-workstudy/list-workstudy.component';

import { WorkstudyService } from "./services/workstudy.service";
import { SelectModule } from 'ng2-select';
import { MyDatePickerModule } from 'mydatepicker';


@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    MenuComponent,
    CreateWorkstudyComponent,
    AssignMaterialComponent,
    HomeComponent,
    LoginComponent,
    ListWorkstudyComponent,
  ],
  imports: [
    routing,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    Angular2FontAwesomeModule,
    DataTablesModule,
    SelectModule,
    MyDatePickerModule 
  ],
  providers: [WorkstudyService],
  bootstrap: [AppComponent]
})
export class AppModule { }
