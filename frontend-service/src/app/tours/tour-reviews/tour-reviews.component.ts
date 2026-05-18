import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReviewService, Review, CreateReviewRequest } from '../../services/review.service';

@Component({
  selector: 'app-tour-reviews',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tour-reviews.component.html',
  styleUrls: ['./tour-reviews.component.css']
})
export class TourReviewsComponent implements OnInit, OnChanges {
  @Input() tourId!: string;

  reviews: Review[] = [];
  isLoading = false;
  error: string | null = null;
  showForm = false;

  rating: number = 5;
  comment: string = '';
  visitDate: string = '';
  imageUrl: string = '';
  images: string[] = [];

  constructor(private reviewService: ReviewService) { }

  ngOnInit(): void {
    if (this.tourId) {
      this.loadReviews();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['tourId']) {
      this.reviews = [];
      this.showForm = false;
      this.error = null;
      if (this.tourId) {
        this.loadReviews();
      }
    }
  }

  loadReviews(): void {
    this.isLoading = true;
    this.error = null;
    this.reviewService.getReviews(this.tourId).subscribe({
      next: (reviews) => {
        this.reviews = reviews;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load reviews';
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
  }

  addImage(): void {
    if (this.imageUrl.trim()) {
      this.images.push(this.imageUrl.trim());
      this.imageUrl = '';
    }
  }

  removeImage(index: number): void {
    this.images.splice(index, 1);
  }

  submitReview(): void {
    if (!this.comment.trim() || !this.visitDate) {
      this.error = 'Please fill in all required fields';
      return;
    }

    const request: CreateReviewRequest = {
      rating: this.rating,
      comment: this.comment,
      visitDate: this.visitDate,
      images: this.images
    };

    this.reviewService.createReview(this.tourId, request).subscribe({
      next: (review) => {
        this.reviews.unshift(review);
        this.resetForm();
        this.showForm = false;
        this.error = null;
      },
      error: (err) => {
        this.error = 'Failed to submit review';
        console.error(err);
      }
    });
  }

  private resetForm(): void {
    this.rating = 5;
    this.comment = '';
    this.visitDate = '';
    this.imageUrl = '';
    this.images = [];
  }

  getRatingStars(rating: number): number[] {
    return Array(Math.round(rating)).fill(0).map((_, i) => i + 1);
  }

  getEmptyStars(rating: number): number[] {
    return Array(5 - Math.round(rating)).fill(0).map((_, i) => i + 1);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}