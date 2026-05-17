import { Component} from '@angular/core';
import { AuthService } from './services/auth.service';
import { UserService } from './services/user.service';
import { RouterOutlet} from '@angular/router';
import { Navbar } from './layout/navbar/navbar';
import 'leaflet-routing-machine';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, Navbar],
  template: `<app-navbar></app-navbar>
    <router-outlet></router-outlet>`
})
export class App {
  constructor(
    private authService: AuthService,
    private userService: UserService,
  ) {}

  ngOnInit() {
    if (this.authService.tokenIsPresent()) {
      this.userService.getMyInfo().subscribe({
        error: () => {
          this.authService.clearSession();
        }
      });
    }
  }

}