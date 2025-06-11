import { Board } from "./Board";

export type Composition = {
  compositionId: number;
  minTicks: number;
  maxPoints: number;
  totalResults: number;
  victoryCount: number;
  victoryRate: number;
  board: Board[];
};