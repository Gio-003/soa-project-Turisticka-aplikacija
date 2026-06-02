import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  constructor(
    private router: Router,
    public userService: UserService
  ) { }

  hasSignedIn(): boolean {
    return !!this.userService.currentUser;
  }

  isTourist(): boolean {
    return this.userService.currentUser?.role === 'TOURIST';
  }

  isGuide(): boolean {
    return this.userService.currentUser?.role === 'GUIDE';
  }

  goTo(path: string): void {
    this.router.navigate([path]);
  }
}
