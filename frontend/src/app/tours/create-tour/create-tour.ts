import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
// 1. Dodaj FormsModule ovde u import listu na vrhu
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { KeyPoint } from '../../shared/map/map.component';

@Component({
  selector: 'app-create-tour',
  standalone: true,
  // 2. Dodaj FormsModule ovde u imports niz komponente
  imports: [CommonModule, ReactiveFormsModule, FormsModule, SharedModule],
  templateUrl: './create-tour.html',
  styleUrl: './create-tour.css',
})
export class CreateTour {
  // ... ostatak tvog TypeScript koda ostaje potpuno isti
  form!: FormGroup;
  submitted = false;
  notification: any;
  
  keyPoints: KeyPoint[] = []; 
  isPointSelected = false;    
  currentPoint: KeyPoint = { lat: 0, lng: 0, name: '', description: '', image: '' }; 

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      name: [''],
      description: [''],
      difficulty: [''],
      tags: ['']
    });
  }

  handlePointSelected(coords: KeyPoint): void {
    this.isPointSelected = true;
    this.currentPoint = {
      lat: coords.lat,
      lng: coords.lng,
      name: '',
      description: '',
      image: ''
    };
  }

  addKeyPoint(): void {
    if (!this.currentPoint.name) {
      alert('Molimo unesite naziv ključne tačke.');
      return;
    }

    this.keyPoints = [...this.keyPoints, { ...this.currentPoint }];
    this.isPointSelected = false;
    this.currentPoint = { lat: 0, lng: 0, name: '', description: '', image: '' };
  }
  
  onSubmit() {
    if (this.form.invalid || this.keyPoints.length === 0) {
      this.notification = {
        msgType: 'error',
        msgBody: 'Form is invalid or you haven\'t added any key points yet.'
      };
      return;
    }

    this.submitted = true;

    const tourData = {
      ...this.form.value,
      keyPoints: this.keyPoints
    };

    console.log('Podaci spremni za backend:', tourData);

    this.notification = {
      msgType: 'success',
      msgBody: 'Tour created successfully!'
    };

    this.submitted = false;
  }
}