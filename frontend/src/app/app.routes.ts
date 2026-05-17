import { Routes } from '@angular/router';
import { Registration} from './stakeholders/registration/registration';
import { LoginComponent } from './stakeholders/login/login';
import { Home } from './layout/home/home';
import { Navbar } from './layout/navbar/navbar';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'navbar', component: Navbar },
  { path: 'signup', component: Registration },
  { path: 'login', component: LoginComponent },
];