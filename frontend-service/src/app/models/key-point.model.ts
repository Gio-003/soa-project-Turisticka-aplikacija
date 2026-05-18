// src/app/models/key-point.model.ts
export interface KeyPoint {
  id?: string; // Guid se mapira u string
  name: string;
  description: string;
  imageUrl: string;
  latitude: number;
  longitude: number;
  tourId?: string;
}