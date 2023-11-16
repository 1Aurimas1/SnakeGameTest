export const GameMode = {
  SOLO: 0,
  DUEL: 1,
} as const;

export type GameMode = (typeof GameMode)[keyof typeof GameMode];
