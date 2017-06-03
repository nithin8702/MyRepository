import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { CreateEmployeeComponent } from './components/employee/create-employee/create-employee.component';
import { EditEmployeeComponent } from './components/employee/edit-employee/edit-employee.component';
import { ListEmployeeComponent } from './components/employee/list-employee/list-employee.component';

const appRoutes: Routes = [
    { path: 'home', component: HomeComponent },
    { path: 'create', component: CreateEmployeeComponent },
    { path: 'edit/:id', component: EditEmployeeComponent },
    { path: 'list', component: ListEmployeeComponent },
];

export const routing = RouterModule.forRoot(appRoutes);
