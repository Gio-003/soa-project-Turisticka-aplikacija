import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ConfigService } from './config.service';

export interface CartItem {
  id: string;
  tourId: string;
  tourName: string;
  price: number;
}

export interface ShoppingCart {
  id: string;
  touristId: number;
  totalPrice: number;
  items: CartItem[];
}

export interface CheckoutResponse {
  createdTokens: number;
  tokens: any[];
}

@Injectable({
  providedIn: 'root'
})
export class PurchaseService {
  constructor(
    private apiService: ApiService,
    private config: ConfigService
  ) {}

  getCart(): Observable<ShoppingCart> {
    return this.apiService.get(this.config.cart_url);
  }

  addToCart(tourId: string): Observable<ShoppingCart> {
    return this.apiService.post(`${this.config.cart_url}/items/${tourId}`, {});
  }

  removeFromCart(tourId: string): Observable<ShoppingCart> {
    return this.apiService.delete(`${this.config.cart_url}/items/${tourId}`);
  }

  checkout(): Observable<CheckoutResponse> {
    return this.apiService.post(`${this.config.cart_url}/checkout`, {});
  }

  getToken(tourId: string): Observable<any> {
    return this.apiService.get(`${this.config.purchases_url}/${tourId}/token`);
  }
}
