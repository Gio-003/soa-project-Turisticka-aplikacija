import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

interface DisplayMessage {
  msgType: string;
  msgBody: string;
}

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './registration.html',
  styleUrls: ['./registration.css'],
})

export class Registration {

  form!: FormGroup;
  submitted = false;
  notification?: DisplayMessage;

  returnUrl = '/';
  passwordStrength: number = 0;

  private ngUnsubscribe: Subject<void> = new Subject<void>();

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder
  ) { }

  ngOnInit(): void {
    console.log('RegistrationComponent init');
    this.route.params
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((params: any) => {
        this.notification = params as DisplayMessage;
      });
    // sigurna inicijalizacija forme
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.form = this.formBuilder.group({
      firstname: ['', Validators.required],
      lastname: ['', Validators.required],
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      role: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordsMatchValidator });

    this.form.get('password')?.valueChanges.subscribe(value => {
      this.checkPasswordStrength(value);
    });
  }

  onSubmit(): void {

    this.notification = undefined;
    this.submitted = true;

    if (this.passwordStrength < 3) {
      this.notification = {
        msgType: 'error',
        msgBody: 'Password is too weak. Use letters, numbers, and a special character.'
      };

      this.submitted = false;
      return;
    }

    const passwordControl = this.form.get('password');


    if (this.form.invalid) {
      const passwordControl = this.form.get('password');
      const emailControl = this.form.get('email');

      if (emailControl?.hasError('required')) {
        this.notification = { msgType: 'error', msgBody: 'Email is required.' };
      } else if (emailControl?.hasError('username')) {
        this.notification = { msgType: 'error', msgBody: 'Invalid email format.' };
      } else if (passwordControl?.hasError('minlength')) {
        this.notification = { msgType: 'error', msgBody: 'Password is too short.' };
      } else if (this.form.errors?.['passwordsMismatch']) {
        this.notification = { msgType: 'error', msgBody: 'Passwords do not match.' };
      } else {
        this.notification = { msgType: 'error', msgBody: 'Check the entered data' };
      }

      this.submitted = false;
      return;
    }

    // ukloni confirmPassword pre slanja na backend
    const { confirmPassword, ...userData } = this.form.value;

    this.authService.signup(userData).subscribe({
      next: () => {
        // Umesto navigacije, prikazujemo poruku
        this.notification = {
          msgType: 'success',
          msgBody: 'Registration successful! You can login now.'
        };

        setTimeout(() => this.router.navigate(['/login']), 3000);
      },
      error: err => {
        this.submitted = false;

        if (err.status === 409) {
          // Konflikt – email ili username već postoji
          this.notification = { msgType: 'error', msgBody: err.error };
        } else {
          this.notification = { msgType: 'error', msgBody: 'An error occurred. Try again.' };
        }
      }
    });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  private passwordsMatchValidator(form: FormGroup) {
    const password = form.get('password')?.value;
    const confirm = form.get('confirmPassword')?.value;
    return password === confirm ? null : { passwordsMismatch: true };
  }

  checkPasswordStrength(password: string) {
    let strength = 0;

    if (!password) {
      this.passwordStrength = 0;
      return;
    }

    if (password.length >= 8) strength++;
    if (/[a-z]/.test(password)) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/\d/.test(password)) strength++;
    if (/[^A-Za-z0-9]/.test(password)) strength++;

    this.passwordStrength = strength;
  }
}
