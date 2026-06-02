import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { switchMap, takeUntil } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';

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
export class LoginComponent implements OnInit, OnDestroy {
  title = 'Login';
  form!: FormGroup;
  submitted = false;
  notification?: DisplayMessage;
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

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/blogs';
    this.form = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(32)])]
    });
  }

  ngOnDestroy() {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  onSubmit() {
    if (this.form.invalid) {
      this.notification = { msgType: 'error', msgBody: 'Username and password are required.' };
      return;
    }

    this.submitted = true;
    this.notification = undefined;

    this.authService.login(this.form.value)
      .pipe(switchMap(() => this.userService.getMyInfo()))
      .subscribe({
        next: () => {
          this.router.navigate([this.returnUrl]);
        },
        error: err => {
          console.log(err);
          this.submitted = false;

          if (err.status === 401) {
            this.notification = { msgType: 'error', msgBody: 'Incorrect username or password.' };
            return;
          }

          this.authService.clearSession();
          this.notification = { msgType: 'error', msgBody: 'Login succeeded, but user profile could not be loaded.' };
        }
      });
  }
}
