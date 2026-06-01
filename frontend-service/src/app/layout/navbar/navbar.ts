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
    const fullName = [user?.firstName, user?.lastName].filter(Boolean).join(' ');
    return fullName || user?.username || `User ${user?.id}`;
  }

  hasSignedIn() {
    return !!this.userService.currentUser;
  }

  goHome(): void {
    this.router.navigate(['/']);
  }

  logout() {
    this.authService.logout();
  }

  goToTourPanel(): void {
    this.router.navigate(['/tours/all']);
  }

  goToBlogs(): void {
    this.router.navigate(['/blogs']);
  }

  goToCart(): void {
    this.router.navigate(['/cart']);
  }
}
