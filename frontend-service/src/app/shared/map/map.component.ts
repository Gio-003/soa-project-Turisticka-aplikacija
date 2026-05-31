import { Component, AfterViewInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import * as L from 'leaflet';
import 'leaflet-routing-machine'; 
import { MapService } from './map.service';

export interface KeyPoint {
  id?: string;
  lat: number;
  lng: number;
  name?: string;
  description?: string;
  image?: string;
}

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css'],
  standalone: false
})
export class MapComponent implements AfterViewInit, OnChanges {
  private map: any;
  private routeControl: any;
  private markers: L.Marker[] = [];

  @Input() mode: 'create' | 'view' | 'edit' = 'view'; 
  @Input() tourPoints: KeyPoint[] = []; 
  @Input() center: [number, number] = [45.2396, 19.8227]; 
  
  @Output() onPointSelected = new EventEmitter<KeyPoint>(); 
  @Output() onPointMoved = new EventEmitter<{ index: number, lat: number, lng: number }>(); 
  @Output() tourLengthChanged = new EventEmitter<number>();

  constructor(private mapService: MapService) {}

  ngAfterViewInit(): void {
    let DefaultIcon = L.icon({
      iconUrl: 'https://unpkg.com/leaflet@1.6.0/dist/images/marker-icon.png',
      iconSize: [25, 41],
      iconAnchor: [12, 41]
    });
    L.Marker.prototype.options.icon = DefaultIcon;
    this.initMap();
  }

  // KLJUČNO: Prati dinamičke promene moda i tačaka iz roditeljske komponente
  ngOnChanges(changes: SimpleChanges): void {
    if (!this.map) return;

    // Ako se promenio mod (npr. iz edit prešlo u view), skidamo stare klikove i osvežavamo lejere
    if (changes['mode']) {
      this.map.off('click'); 
      if (this.mode === 'create' || this.mode === 'edit') {
        this.registerOnClick();
      }
      this.refreshMapLayers();
    } 
    // Ako su se promenile same tačke
    else if (changes['tourPoints']) {
      this.refreshMapLayers();
    }
  }

  private initMap(): void {
    this.map = L.map('map', {
      center: this.center,
      zoom: 13,
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 18,
      minZoom: 3,
      attribution: '&copy; OpenStreetMap contributors',
    }).addTo(this.map);

    if (this.mode === 'create' || this.mode === 'edit') {
      this.registerOnClick();
    }
    
    if (this.tourPoints.length > 0) {
      this.displayTour();
    }
  }

  registerOnClick(): void {
    this.map.on('click', (e: any) => {
      if (this.mode !== 'create' && this.mode !== 'edit') return;
      
      const coord = e.latlng;
      this.onPointSelected.emit({ lat: coord.lat, lng: coord.lng });
    });
  }

  private refreshMapLayers(): void {
    this.clearMarkers();
    if (this.routeControl) {
      this.map.removeControl(this.routeControl);
      this.routeControl = null;
    }
    if (this.tourPoints.length > 0) {
      this.displayTour();
    }
  }

  private displayTour(): void {
    this.tourPoints.forEach((point, index) => {
      const popupContent = `
        <strong>${point.name || 'Ključna tačka'}</strong><br>
        ${point.description || ''}
      `;

      // STROGO KONTROLISANO DRAGGABLE STANJE: samo i isključivo ako je mode 'edit'
      const isDraggable = this.mode === 'edit';

      const marker = L.marker([point.lat, point.lng], { draggable: isDraggable })
        .addTo(this.map)
        .bindPopup(popupContent);
        
      if (isDraggable) {
        marker.on('dragend', (event: any) => {
          const newLatLng = event.target.getLatLng();
          this.onPointMoved.emit({
            index: index,
            lat: newLatLng.lat,
            lng: newLatLng.lng
          });
        });
      }
        
      this.markers.push(marker);
    });

    if (this.tourPoints.length > 1) {
      this.setRoute(this.tourPoints);
    }
  }

setRoute(points: KeyPoint[]): void {
  if (this.routeControl) {
    try {
      this.map.removeControl(this.routeControl);
    } catch (e) {}
    this.routeControl = null;
  }

  const waypoints = points.map(p => L.latLng(p.lat, p.lng));

  this.routeControl = (L.Routing as any).control({
    waypoints: waypoints,
    addWaypoints: false,
    draggableWaypoints: false,
    show: false
  }).addTo(this.map);

  this.routeControl.on('routesfound', (e: any) => {
    const route = e.routes[0];

    // distance je u metrima
    const distanceKm = route.summary.totalDistance / 1000;

    console.log('Dužina ture:', distanceKm);

    this.tourLengthChanged.emit(distanceKm);
  });
}