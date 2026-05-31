import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; 
import { SharedModule } from '../../shared/shared.module';
import { TourService } from '../../services/tour.service';
import { KeyPointService } from '../../services/key-point.service';
import { KeyPoint as ApiKeyPoint } from '../../models/key-point.model'; // Model sa backenda
import { KeyPoint as MapKeyPoint } from '../../shared/map/map.component'; // Model za mapu
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { TourReviewsComponent } from '../tour-reviews/tour-reviews.component';

// Popravljen interfejs: Uklonjen neispravan 'extends ApiKeyPoint'
export interface Tour {
  id: string;
  name: string;
  description: string;
  difficulty: string;
  tags: string[];
  keyPoints: ApiKeyPoint[];
  lengthInKm: number; // Dodato polje za dužinu ture
}

@Component({
  selector: 'app-all-tours',
  standalone: true,
  imports: [CommonModule, FormsModule, SharedModule, TourReviewsComponent],
  templateUrl: './all-tours.html',
  styleUrls: ['./all-tours.css'],
})
export class AllToursComponent implements OnInit {
  tours: Tour[] = [];
  selectedTour: Tour | null = null;
  
  // Niz ključnih tačaka formatiran za mapu
  mapKeyPoints: MapKeyPoint[] = [];
  mapMode: 'create' | 'view' | 'edit' = 'view';

  // Stanja za formu izmene
  isEditingKeyPoint = false;
  isFormVisible = false;
  currentPoint: MapKeyPoint = { lat: 0, lng: 0, name: '', description: '', image: '' };
  editingKeyPointId: string | null = null; // ID tačke koja se menja
  tourLength = 0;

  constructor(
    private tourService: TourService,
    private keyPointService: KeyPointService,
    public userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTours();
  }

  goToCreateTour(): void {
    this.router.navigate(['/tours/create']);
  }

  goToMyTours(): void {
    this.router.navigate(['/tours/my']);
  }

  hasSignedIn() {
    return !!this.userService.currentUser;
  }

  loadTours(): void {
    this.tourService.getAllTours().subscribe((tours: Tour[]) => {
      this.tours = tours;
    });
  }

  selectTour(tour: Tour): void {
    this.selectedTour = tour;
    this.isEditingKeyPoint = false;
    this.isFormVisible = false;
    this.mapMode = 'view';
    this.tourLength = tour.lengthInKm;
    // Mapiramo ApiKeyPoint u MapKeyPoint
    this.mapKeyPoints = tour.keyPoints.map(kp => ({
      id: kp.id,
      lat: kp.latitude,
      lng: kp.longitude,
      name: kp.name,
      description: kp.description,
      image: kp.imageUrl,
    }));
  }

  // --- LOGIKA ZA IZMENU I BRISANJE ---

  // 1. Korisnik klikne na "Izmeni" pored neke tačke
  startEditKeyPoint(kp: MapKeyPoint): void {
    this.isEditingKeyPoint = true;
    this.isFormVisible = true;
    this.mapMode = 'edit'; // Palimo edit mod na mapi da markeri postanu pokretni
    this.editingKeyPointId = kp.id!;
    this.currentPoint = { ...kp }; 
  }

  // Klik na "+ Add New Key Point" dugme iz HTML-a
  startAddNewKeyPoint(): void {
    this.isFormVisible = true;
    this.isEditingKeyPoint = false;
    this.mapMode = 'edit'; // Prebacujemo mapu u edit mod da korisnik može klikom da izabere poziciju
    this.currentPoint = { lat: 0, lng: 0, name: '', description: '', image: '' };
    this.editingKeyPointId = null;
  }

  // 2. Korisnik klikne na "Obriši"
  deleteKeyPoint(kp: MapKeyPoint): void {
    if (!this.selectedTour || !kp.id) return;
    if (confirm(`Are you sure you want to delete key point: ${kp.name}?`)) {
      this.keyPointService.deleteKeyPoint(this.selectedTour.id, kp.id).subscribe(() => {
        // Uklanjamo tačku iz lokalnih nizova
        this.selectedTour!.keyPoints = this.selectedTour!.keyPoints.filter(p => p.id !== kp.id);
        this.mapKeyPoints = this.mapKeyPoints.filter(p => p.id !== kp.id);
      });
    }
  }

  // 3. Reagovanje na novi klik na mapi (Hvatanje koordinata)
  handlePointSelected(coords: MapKeyPoint): void {
    if (this.mapMode !== 'edit') return; // Ne dozvoljavamo klik ako nismo otvorili formu
    
    this.currentPoint.lat = coords.lat;
    this.currentPoint.lng = coords.lng;
  }

  // 3b. Reagovanje na prevlačenje (drag) postojećeg markera na mapi
 handlePointMoved(event: { index: number, lat: number, lng: number }): void {
    if (this.mapMode !== 'edit') return;

    const movedMarker = this.mapKeyPoints[event.index];

    // Ako pomeramo baš onu tačku koja nam je trenutno otvorena u formi za izmenu
    if (this.isEditingKeyPoint && movedMarker.id === this.editingKeyPointId) {
      this.currentPoint.lat = event.lat;
      this.currentPoint.lng = event.lng;
    }

    // Uvek pravimo novu referencu niza da MapComponent osvezi rutu
    this.mapKeyPoints = this.mapKeyPoints.map((p, idx) => {
      if (idx !== event.index) return p;
      return {
        ...p,
        lat: event.lat,
        lng: event.lng
      };
    });
  }

  // 4. Potvrda izmene ili dodavanja nove tačke preko forme
  saveKeyPoint(): void {
    if (!this.selectedTour || !this.currentPoint.name) {
      alert('Key point name is required.');
      return;
    }

    const payload = {
      name: this.currentPoint.name || '',
      description: this.currentPoint.description || '',
      imageUrl: this.currentPoint.image || '',
      latitude: this.currentPoint.lat,
      longitude: this.currentPoint.lng,
    };

    if (this.isEditingKeyPoint && this.editingKeyPointId) {
      // --- IZMENA POSTOJEĆE TAČKE ---
      this.keyPointService.updateKeyPoint(this.selectedTour.id, this.editingKeyPointId, payload)
        .subscribe(updatedKp => {
          // 1. Ažuriramo bekraund model ture
          
          const apiIndex = this.selectedTour!.keyPoints.findIndex(p => p.id === this.editingKeyPointId);
          if (apiIndex > -1) this.selectedTour!.keyPoints[apiIndex] = updatedKp;

          // 2. KLJUČNI DEO: Mapiramo niz ponovo stvarajući potpuno nove reference objekata!
          this.mapKeyPoints = this.mapKeyPoints.map(p => {
            if (p.id === this.editingKeyPointId) {
              return {
                id: updatedKp.id,
                lat: this.currentPoint.lat,
                lng: this.currentPoint.lng,
                name: this.currentPoint.name,
                description: this.currentPoint.description,
                image: this.currentPoint.image
              };
            }
            return p;
          });

          this.resetForm();
        });
    } else {
      // --- DODAVANJE NOVE TAČKE ---
      this.keyPointService.addKeyPoint(this.selectedTour.id, payload)
        .subscribe(newKp => {
          this.selectedTour?.keyPoints.push(newKp);
 
          this.mapKeyPoints = [...this.mapKeyPoints, {
            id: newKp.id,
            lat: newKp.latitude,
            lng: newKp.longitude,
            name: newKp.name,
            description: newKp.description,
            image: newKp.imageUrl
          }];
          this.resetForm();
        });
    }
  }

  cancelEdit(): void {
    this.resetForm();
  }

  private resetForm(): void {
    this.isEditingKeyPoint = false;
    this.isFormVisible = false;
    this.mapMode = 'view'; // Vraćamo mapu u običan pregled
    this.editingKeyPointId = null;
    this.currentPoint = { lat: 0, lng: 0, name: '', description: '', image: '' };
  }
  onTourLengthChanged(length: number) {
  this.tourLength = length;

  if (this.selectedTour) {
    this.selectedTour.lengthInKm = length;

    this.tourService
      .updateTourLength(this.selectedTour.id, length)
      .subscribe();
  }
}

}