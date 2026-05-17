import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-create-tour',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-tour.html',
  styleUrl: './create-tour.css',
})
export class CreateTour {

  form!: FormGroup;
  submitted = false;
  notification: any;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      name: [''],
      description: [''],
      difficulty: [''],
      tags: ['']
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    console.log(this.form.value);

    this.submitted = true;

    this.notification = {
      msgType: 'success',
      msgBody: 'Tour created'
    };

    this.submitted = false;
  }
}