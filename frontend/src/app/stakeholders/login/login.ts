import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import {Subject} from 'rxjs';
import {takeUntil} from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

interface DisplayMessage {
  msgType: string;
  msgBody: string;
}

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class LoginComponent implements OnInit {
  title = 'Login';
  form!: FormGroup;

  /**
   * Boolean used in telling the UI
   * that the form has been submitted
   * and is awaiting a response
   */
  submitted = false;

   /**
   * Notification message from received
   * form request or router
   */
    notification!: DisplayMessage;

    returnUrl!: string;
    private ngUnsubscribe: Subject<void> = new Subject<void>();


  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder
  ) { }

  ngOnInit() {
    this.route.params
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((params: any) => {
        this.notification = params as DisplayMessage;
      });
    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.form = this.formBuilder.group({
      username: ['', Validators.compose([Validators.required, Validators.email])],
      password: ['', Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(32)])]
    });
  }

  ngOnDestroy() {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  onSubmit() {
  this.notification;
  this.submitted = true;

  this.authService.login(this.form.value)
    .subscribe({
      next: data => {
        console.log(data);
        this.userService.getMyInfo().subscribe();
        this.router.navigate(['/']);
      },
      error: err => {
        console.log(err);
        this.submitted = false;

        if (err.status === 401) {
          this.notification = {msgType: 'error', msgBody: 'Incorrect email or password..'};
        } else {
          // Ostale greške
          this.notification = {msgType: 'error', msgBody: 'An error occurred. Try again.'};
        }
      }
    });
}

}