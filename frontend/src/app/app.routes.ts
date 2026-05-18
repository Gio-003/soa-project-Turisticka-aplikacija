import { Routes } from '@angular/router';
import { Registration} from './stakeholders/registration/registration';
import { LoginComponent } from './stakeholders/login/login';
import { Home } from './layout/home/home';
import { Navbar } from './layout/navbar/navbar';
import { CreateTour } from './tours/create-tour/create-tour';
import { AllToursComponent } from './tours/all-tours/all-tours';
import { TourDetailComponent } from './tours/tour-detail/tour-detail';
import { CreateReviewComponent } from './tours/create-review/create-review';
import { ReviewListComponent } from './tours/review-list/review-list';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'navbar', component: Navbar },
  { path: 'signup', component: Registration },
  { path: 'login', component: LoginComponent },
  { path: 'tours/create', component: CreateTour },
  { path: 'tours/all', component: AllToursComponent },
  { path: 'tours/:tourId', component: TourDetailComponent },
  { path: 'tours/:tourId/reviews', component: ReviewListComponent },
  { path: 'tours/:tourId/review/create', component: CreateReviewComponent },
];