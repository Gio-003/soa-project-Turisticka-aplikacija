import { Component, AfterViewInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import * as L from 'leaflet';
import 'leaflet-routing-machine'; // Obavezno uvezi routing plugin ako već nisi
import { MapService } from './map.service';

// Definišemo interfejs za ključnu tačku radi lakšeg rada sa podacima
export interface KeyPoint {
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

  // --- INPUTI (Podaci koje komponenta prima od roditelja) ---
  @Input() mode: 'create' | 'view' = 'create'; // Mod rada komponente
  @Input() tourPoints: KeyPoint[] = []; // Tačke koje treba prikazati u 'view' modu

  // --- OUTPUTI (Događaji koje komponenta šalje roditelju) ---
  @Input() center: [number, number] = [45.2396, 19.8227]; // Default: Novi Sad
  @Output() onPointSelected = new EventEmitter<KeyPoint>(); // Šalje kliknutu lokaciju

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

  // Prati promene na @Input poljima (ako roditelj naknadno pošalje nove tačke)
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['tourPoints'] && this.map) {
      // Prvo brišemo stare markere sa mape
      this.clearMarkers();
      
      // Ako ruter već postoji, sklonimo ga pre iscrtavanja novog rute
      if (this.routeControl) {
        this.map.removeControl(this.routeControl);
      }

      if (this.tourPoints.length > 0) {
        this.displayTour();
      }
    }
  }
  private initMap(): void {
    this.map = L.map('map', {
      center: this.center,
      zoom: 13,
    });

    const tiles = L.tileLayer(
      'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
      {
        maxZoom: 18,
        minZoom: 3,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>',
      }
    );
    tiles.addTo(this.map);

    // Aktiviraj klik na mapu samo ako smo u modu za kreiranje tačaka
    if (this.mode === 'create') {
      this.registerOnClick();
    } else if (this.mode === 'view' && this.tourPoints.length > 0) {
      this.displayTour();
    }
  }

  // --- METODA ZA REGISTRACIJU KLIKA (Kreiranje tačke) ---
  registerOnClick(): void {
    this.map.on('click', (e: any) => {
      const coord = e.latlng;
      
      // Brišemo prethodni privremeni marker ako postoji
      this.clearMarkers();

      // Postavljamo novi marker na mesto klika
      const marker = new L.Marker([coord.lat, coord.lng]).addTo(this.map);
      this.markers.push(marker);

      // Šaljemo koordinate roditeljskoj komponenti
      this.onPointSelected.emit({
        lat: coord.lat,
        lng: coord.lng
      });
    });
  }

  // --- PARAMETRIZOVANA METODA ZA RUTU ---
  setRoute(points: KeyPoint[]): void {
    // Ako već postoji stara ruta na mapi, ukloni je
    if (this.routeControl) {
      this.map.removeControl(this.routeControl);
    }

    // Pretvaramo naše tačke u L.latLng objekte koje Leaflet Routing prihvata
    const waypoints = points.map(p => L.latLng(p.lat, p.lng));

    this.routeControl = L.Routing.control({
      waypoints: waypoints,
     
      addWaypoints: false, // Korisnik ne može sam da prevlači i dodaje tačke na ruti
    }).addTo(this.map);

    this.routeControl.on('routesfound', (e: any) => {
      const routes = e.routes;
      const summary = routes[0].summary;
      console.log('Udaljenost: ' + summary.totalDistance / 1000 + ' km, Vreme: ' + Math.round(summary.totalTime / 60) + ' min');
    });
  }

  // Prikazuje sve tačke ture i spaja ih rutom
  private displayTour(): void {
    this.tourPoints.forEach(point => {
      const popupContent = `
        <strong>${point.name || 'Ključna tačka'}</strong><br>
        ${point.description || ''}<br>
        ${point.image ? `<img src="${point.image}" width="100" style="margin-top:5px; border-radius:4px;" />` : ''}
      `;

      const marker = L.marker([point.lat, point.lng])
        .addTo(this.map)
        .bindPopup(popupContent);
        
      this.markers.push(marker);
    });

    // Ako imamo više od 1 tačke, iscrtaj rutu između njih
    if (this.tourPoints.length > 1) {
      this.setRoute(this.tourPoints);
    }

    // Centriraj mapu oko prve tačke na turi
    this.map.setView([this.tourPoints[0].lat, this.tourPoints[0].lng], 13);
  }

  // Pomoćne metode za čišćenje mape
  private clearMarkers(): void {
    this.markers.forEach(m => this.map.removeLayer(m));
    this.markers = [];
  }

  private clearMap(): void {
    this.clearMarkers();
    if (this.routeControl) {
      this.map.removeControl(this.routeControl);
    }
  }

  // Zadržana metoda za pretragu iz tvog koda, sada je dinamička
  search(address: string): void {
    this.mapService.search(address).subscribe({
      next: (result) => {
        if(result && result.length > 0) {
          const lat = result[0].lat;
          const lon = result[0].lon;
          this.map.setView([lat, lon], 15);
          L.marker([lat, lon]).addTo(this.map).bindPopup(address).openPopup();
        }
      },
      error: () => {},
    });
  }
}