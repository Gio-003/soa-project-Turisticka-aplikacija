import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReviewService } from '../../services/review.service';
import { ActivatedRoute } from '@angular/router';

interface DisplayMessage {
  msgType: string;
  msgBody: string;
}

@Component({
  selector: 'app-create-review',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './create-review.html',
  styleUrls: ['./create-review.css']
})
export class CreateReviewComponent implements OnInit {
  form!: FormGroup;
  tourId: string = '';
  submitted = false;
  notification?: DisplayMessage;
  selectedImages: File[] = [];
  imagePreviews: string[] = [];

  constructor(
    private fb: FormBuilder,
    private reviewService: ReviewService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.tourId = this.route.snapshot.paramMap.get('tourId') || '';

    this.form = this.fb.group({
      rating: ['', [Validators.required, Validators.min(1), Validators.max(5)]],
      comment: ['', [Validators.required, Validators.minLength(10)]],
      visitDate: ['', Validators.required]
    });
  }

  onImageSelected(event: any): void {
    const files: File[] = Array.from(event.target.files);
    
    files.forEach(file => {
      if (file.type.startsWith('image/')) {
        this.selectedImages.push(file);
        
        // Preview slike
        const reader = new FileReader();
        reader.onload = (e: any) => {
          this.imagePreviews.push(e.target.result);
        };
        reader.readAsDataURL(file);
      }
    });
  }

  removeImage(index: number): void {
    this.selectedImages.splice(index, 1);
    this.imagePreviews.splice(index, 1);
  }

  onSubmit(): void {
    this.notification = undefined;
    
    if (this.form.invalid) {
      this.notification = {
        msgType: 'error',
        msgBody: 'Molimo popunite sve obavezne poljeuke.'
      };
      return;
    }

    this.submitted = true;

    // Konverzija slika u base64
    const imagePromises = this.selectedImages.map(file => 
      new Promise<string>((resolve) => {
        const reader = new FileReader();
        reader.onload = (e: any) => {
          resolve(e.target.result);
        };
        reader.readAsDataURL(file);
      })
    );

    Promise.all(imagePromises).then(base64Images => {
      const formValue = this.form.value;

      const request = {
        rating: parseInt(formValue.rating),
        comment: formValue.comment,
        visitDate: formValue.visitDate,
        images: base64Images
      };

      this.reviewService.createReview(this.tourId, request).subscribe({
        next: (response) => {
          this.notification = {
            msgType: 'success',
            msgBody: 'Recenzija je uspešno objavljena!'
          };
          this.form.reset();
          this.selectedImages = [];
          this.imagePreviews = [];
          this.submitted = false;
        },
        error: (error) => {
          this.notification = {
            msgType: 'error',
            msgBody: error.error?.error || 'Greška pri objavljivanju recenzije'
          };
          this.submitted = false;
        }
      });
    });
  }
}
