import { Routes, RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { ListWorkstudyComponent } from './components/workstudy/list-workstudy/list-workstudy.component';
import { CreateWorkstudyComponent } from './components/workstudy/create-workstudy/create-workstudy.component'
import { AssignMaterialComponent } from './components/workstudy/assign-material/assign-material.component'
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';

const appRoutes: Routes = [
    //{ path: '', component: CreateWorkstudyComponent },
    // { path: 'login', component: LoginComponent },
    // { path: 'home', component: HomeComponent },
    { path: 'listworkstudy', component: ListWorkstudyComponent },
    { path: 'createworkstudy', component: CreateWorkstudyComponent },
    { path: 'assignmaterial', component: AssignMaterialComponent },
];

export const routing = RouterModule.forRoot(appRoutes);
