import { Routes, RouterModule } from '@angular/router';

import { AddEmployeeComponent } from './employee/add.component';
import { EditEmployeeComponent } from './employee/edit.component';

const appRoutes: Routes = [
    { path: 'add', component: AddEmployeeComponent },
    { path: 'edit', component: EditEmployeeComponent }
];

export const routing = RouterModule.forRoot(appRoutes);