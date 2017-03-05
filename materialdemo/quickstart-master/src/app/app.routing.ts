import { Routes, RouterModule } from '@angular/router';

import { CreateEmployeeComponent } from './components/employee/create.component';
import { EditEmployeeComponent } from './components/employee/edit.component';
import { ListEmployeeComponent } from './components/employee/list.component';

const appRoutes: Routes = [
    { path: 'add', component: CreateEmployeeComponent },
    { path: 'edit', component: EditEmployeeComponent },
    { path: 'list', component: ListEmployeeComponent },
];

export const routing = RouterModule.forRoot(appRoutes);