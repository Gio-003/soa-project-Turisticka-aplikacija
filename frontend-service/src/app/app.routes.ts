import { Routes } from '@angular/router';
import { Registration} from './stakeholders/registration/registration';
import { LoginComponent } from './stakeholders/login/login';
import { Home } from './layout/home/home';
import { Navbar } from './layout/navbar/navbar';
import { CreateTour } from './tours/create-tour/create-tour';
import { AllToursComponent } from './tours/all-tours/all-tours';
import { MyTours } from './tours/my-tours/my-tours';
import { BlogComponent } from './blog/blog/blog.component';
import { CartComponent } from './purchase/cart/cart.component';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'navbar', component: Navbar },
  { path: 'signup', component: Registration },
  { path: 'login', component: LoginComponent },
  { path: 'tours/create', component: CreateTour },
  { path: 'tours/all', component: AllToursComponent },
  { path: 'tours/my', component: MyTours},
  { path: 'cart', component: CartComponent },
  {path: 'blogs', component: BlogComponent}
];
