export interface Point {
  x: number;
  y: number;
}

export interface GameDto {
  playerName: string;
  snakeParts: Point[];
  score: number;
  food?: Point;
}
