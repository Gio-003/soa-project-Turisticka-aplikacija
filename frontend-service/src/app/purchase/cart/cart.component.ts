import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PurchaseService, ShoppingCart } from '../../services/purchase.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cart: ShoppingCart | null = null;
  message = '';
  error = '';
  isCheckingOut = false;

  constructor(
    private purchaseService: PurchaseService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.purchaseService.getCart().subscribe({
      next: cart => {
        this.cart = cart;
        this.error = '';
      },
      error: err => {
        this.error = err?.error?.error || 'Failed to load shopping cart.';
      }
    });
  }

  remove(tourId: string): void {
    this.purchaseService.removeFromCart(tourId).subscribe({
      next: cart => {
        this.cart = cart;
        this.message = 'Item removed from cart.';
      },
      error: err => {
        this.error = err?.error?.error || 'Failed to remove item.';
      }
    });
  }

  checkout(): void {
    if (!this.cart || this.cart.items.length === 0) {
      return;
    }

    this.isCheckingOut = true;
    this.purchaseService.checkout().subscribe({
      next: response => {
        this.isCheckingOut = false;
        this.message = `Checkout completed. Created ${response.createdTokens} purchase token(s).`;
        this.loadCart();
      },
      error: err => {
        this.isCheckingOut = false;
        this.error = err?.error?.error || 'Checkout failed.';
      }
    });
  }

  backToTours(): void {
    this.router.navigate(['/tours/all']);
  }
}
