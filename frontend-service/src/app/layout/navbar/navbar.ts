import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule],
  templateUrl: './navbar.html',
  standalone: true,
  styleUrl: './navbar.css',
})
export class Navbar {
  constructor(
    private router: Router,
    public userService: UserService,
    private authService: AuthService,
  ) { }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }

  goToSignup(): void {
    this.router.navigate(['/signup']);
  }

  userName() {
    const user = this.userService.currentUser;
    return user.firstName + ' ' + user.lastName;
  }

  hasSignedIn() {
    return !!this.userService.currentUser;
  }

  goHome(): void {
    this.router.navigate(['/videos']); // Pretpostavka da je ruta '/videos'
  }

  logout() {
    this.authService.logout();
  }

  goToTourPanel(): void {
    this.router.navigate(['/tours/all']);
  }
}
