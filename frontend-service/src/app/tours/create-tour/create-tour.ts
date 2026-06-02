import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { KeyPoint } from '../../shared/map/map.component';
import { TourService } from '../../services/tour.service';
import { UserService } from '../../services/user.service';

export interface TourDuration {
  transportType: 'Walking' | 'Bicycle' | 'Car';
  durationInMinutes: number;
}

export interface CreateTourRequest {
  name: string;
  description: string;
  difficulty: string;
  tags: string[];
  keyPoints: KeyPoint[];
  durations: TourDuration[];   // NOVO
  authorId: number;
  lengthInKm: number;
  price: number;
}
@Component({
  selector: 'app-create-tour',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, SharedModule],
  templateUrl: './create-tour.html',
  styleUrls: ['./create-tour.css']
})
export class CreateTour implements OnInit {
  form!: FormGroup;
  submitted = false;
  notification: any;

  keyPoints: KeyPoint[] = [];
  isPointSelected = false;
  currentPoint: KeyPoint = { lat: 0, lng: 0, name: '', description: '', image: '' };

  durations: TourDuration[] = [];

  newDuration: TourDuration = {
    transportType: 'Walking',
    durationInMinutes: 0
  };

  constructor(
    private fb: FormBuilder,
    private tourService: TourService,
    public userService: UserService
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      difficulty: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      tags: [''],
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

  addDuration(): void {
    const minutes = Number(this.newDuration.durationInMinutes);

    if (!minutes || minutes <= 0) {
      alert('Unesite validno vreme');
      return;
    }

    const durationToAdd: TourDuration = {
      transportType: this.newDuration.transportType,
      durationInMinutes: minutes
    };

    this.durations = [...this.durations, durationToAdd];

    this.newDuration = {
      transportType: this.newDuration.transportType,
      durationInMinutes: 0
    };
  }

  onSubmit(): void {
    // Provera validnosti forme I da li postoji bar jedna ključna tačka
    if (this.form.invalid || this.keyPoints.length === 0) {
      this.notification = {
        msgType: 'error',
        msgBody: 'Form is invalid or you haven\'t added any key points yet.'
      };
      return;
    }

    this.submitted = true;
    const formValue = this.form.value;

    // Mapiramo podatke i parsiramo tagove u niz stringova
    const request: CreateTourRequest = {
      name: formValue.name,
      description: formValue.description,
      difficulty: formValue.difficulty,
      tags: formValue.tags
        ? formValue.tags.split(',').map((t: string) => t.trim())
        : [],
      keyPoints: this.keyPoints, // Sakupljene tačke sa mape ubacujemo u zahtev
      durations: this.durations,
      authorId: this.userService.currentUser.id,
      lengthInKm: this.tourLength,
      price: Number(formValue.price)
    };

    // Slanje na backend preko servisa
    this.tourService.createTour(request).subscribe({
      next: () => {
        this.notification = {
          msgType: 'success',
          msgBody: 'Tour created successfully!'
        };

        // Resetujemo formu i praznimo tačke sa mape nakon uspešnog čuvanja
        this.form.reset();
        this.keyPoints = [];
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
  tourLength = 0;

onTourLengthChanged(length: number) {
  this.tourLength = length;
}
}
