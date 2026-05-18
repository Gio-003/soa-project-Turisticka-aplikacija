export interface TourResponse {
  id: string;
  name: string;
  description: string;
  difficulty: string;
  price: number;
  status: string;
  authorId: number;
  tags: string[];
}