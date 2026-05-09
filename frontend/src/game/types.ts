export type GameStatus = "idle" | "playing" | "finished";

export type InputState = {
  up: boolean;
  down: boolean;
  left: boolean;
  right: boolean;
};

export type Player = {
  x: number;
  y: number;
  size: number;
  speed: number;
};

export type Orb = {
  x: number;
  y: number;
  radius: number;
};

export type GameState = {
  width: number;
  height: number;
  player: Player;
  orb: Orb;
  score: number;
  timeLeft: number;
  status: GameStatus;
};

export type ScoreRecord = {
  name: string;
  score: number;
  submitted_at: string;
};

