export interface TourDuration {
  id: string;
  tourId: string;
  transportType: 'Walking' | 'Bicycle' | 'Car';
  durationInMinutes: number;
}