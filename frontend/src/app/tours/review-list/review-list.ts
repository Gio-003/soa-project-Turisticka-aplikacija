import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReviewService } from '../../services/review.service';

interface Review {
  id: string;
  tourId: string;
  reviewerUserId: string;
  rating: number;
  comment: string;
  visitDate: string;
  reviewDate: string;
  images: string[];
  reviewerName: string;
  reviewerProfilePicture: string;
}

@Component({
  selector: 'app-review-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './review-list.html',
  styleUrls: ['./review-list.css']
})
export class ReviewListComponent implements OnInit {
  @Input() tourId: string = '';
  
  reviews: Review[] = [];
  stats: { count: number; averageRating: number } | null = null;
  loading = true;
  Math = Math; // Dodaj Math za template

  constructor(private reviewService: ReviewService) {}

  ngOnInit(): void {
    if (this.tourId) {
      this.loadReviews();
      this.loadStats();
    }
  }

  loadReviews(): void {
    this.reviewService.getReviewsByTour(this.tourId).subscribe({
      next: (data) => {
        this.reviews = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading reviews:', err);
        this.loading = false;
      }
    });
  }

  loadStats(): void {
    this.reviewService.getReviewStats(this.tourId).subscribe({
      next: (data) => {
        this.stats = data;
      },
      error: (err) => {
        console.error('Error loading stats:', err);
      }
    });
  }

  getRatingStars(rating: number): string {
    return '⭐'.repeat(rating);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('sr-RS');
  }
}
