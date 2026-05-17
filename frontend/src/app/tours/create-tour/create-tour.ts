import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TourService } from '../../services/tour.service';

export interface CreateTourRequest {
  name: string;
  description: string;
  difficulty: string;
  tags: string[];
}

@Component({
  selector: 'app-create-tour',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-tour.html',
  styleUrls: ['./create-tour.css']
})
export class CreateTour implements OnInit {

  form!: FormGroup;
  submitted = false;

  notification: any = null;

  constructor(
    private fb: FormBuilder,
    private tourService: TourService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      difficulty: ['', Validators.required],
      tags: ['']
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.submitted = true;

    const formValue = this.form.value;

    const request: CreateTourRequest = {
      name: formValue.name,
      description: formValue.description,
      difficulty: formValue.difficulty,
      tags: formValue.tags
        ? formValue.tags.split(',').map((t: string) => t.trim())
        : []
    };

    this.tourService.createTour(request).subscribe({
      next: () => {
        this.notification = {
          msgType: 'success',
          msgBody: 'Tour created successfully!'
        };

        this.form.reset();
        this.submitted = false;
      },
      error: () => {
        this.notification = {
          msgType: 'error',
          msgBody: 'Failed to create tour'
        };

        this.submitted = false;
      }
    });
  }
}