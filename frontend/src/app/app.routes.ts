import { Routes } from '@angular/router';
import { Registration} from './stakeholders/registration/registration';
import { LoginComponent } from './stakeholders/login/login';
import { Home } from './layout/home/home';
import { Navbar } from './layout/navbar/navbar';
import { CreateTour } from './tours/create-tour/create-tour';
import { AllTours } from './tours/all-tours/all-tours';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'navbar', component: Navbar },
  { path: 'signup', component: Registration },
  { path: 'login', component: LoginComponent },
  { path: 'tours/create', component: CreateTour },
  { path: 'tours/all', component: AllTours },
];