import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { ApiService } from './api.service';
import {ConfigService} from './config.service';
import { catchError, map } from 'rxjs/operators';
import { UserService } from './user.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

    constructor(
    private apiService: ApiService,
    private config: ConfigService,
    private userService: UserService,
    private router: Router
  ) {
  }

    private access_token = null;

    signup(user:any) {
    const signupHeaders = new HttpHeaders({
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    });
    return this.apiService.post(this.config.signup_url, JSON.stringify(user), signupHeaders)
      .pipe(map(() => {
        console.log('Sign up success');
      }));
    }

    login(user:any) {
    const loginHeaders = new HttpHeaders({
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    });
     //const body = `username=${user.username}&password=${user.password}`;
    const body = {
      'username': user.username,
      'password': user.password
    };
    return this.apiService.post(this.config.login_url, JSON.stringify(body), loginHeaders)
      .pipe(map((res) => {
        console.log('Login success');
        //this.access_token = res.body.accessToken;
        this.access_token = res.accessToken;
        //localStorage.setItem("jwt", res.body.accessToken);
        localStorage.setItem("jwt_token", res.accessToken);
        return res
      }));
    }

    logout() {
      this.clearSession();
      this.router.navigate(['/login']);
    }

    clearSession() {
      this.userService.currentUser = null;
      localStorage.removeItem("jwt_token");
      this.access_token = null;
    }

    tokenIsPresent() {
      return !!this.getToken();
    }

    getToken() {
      const token = localStorage.getItem("jwt_token");
      if (token === "null" || token === "undefined") return null;
      return token;
    }

    isLoggedIn() {
      return this.tokenIsPresent();
    }
}