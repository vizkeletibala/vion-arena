import {
  GAME_HEIGHT,
  GAME_WIDTH,
  ORB_PADDING,
  ORB_RADIUS,
  PLAYER_SIZE,
  PLAYER_SPEED,
  ROUND_DURATION_SECONDS,
} from "./constants";
import type { GameState, InputState, Orb, Player } from "./types";

export const EMPTY_INPUT: InputState = {
  up: false,
  down: false,
  left: false,
  right: false,
};

export function clamp(value: number, min: number, max: number): number {
  return Math.min(Math.max(value, min), max);
}

function randomInRange(min: number, max: number, randomFn: () => number): number {
  return min + randomFn() * (max - min);
}

export function spawnOrb(
  width: number,
  height: number,
  radius = ORB_RADIUS,
  randomFn: () => number = Math.random,
): Orb {
  return {
    x: randomInRange(ORB_PADDING, width - ORB_PADDING, randomFn),
    y: randomInRange(ORB_PADDING, height - ORB_PADDING, randomFn),
    radius,
  };
}

export function createInitialPlayer(): Player {
  return {
    x: GAME_WIDTH / 2,
    y: GAME_HEIGHT / 2,
    size: PLAYER_SIZE,
    speed: PLAYER_SPEED,
  };
}

export function createInitialGameState(randomFn: () => number = Math.random): GameState {
  return {
    width: GAME_WIDTH,
    height: GAME_HEIGHT,
    player: createInitialPlayer(),
    orb: spawnOrb(GAME_WIDTH, GAME_HEIGHT, ORB_RADIUS, randomFn),
    score: 0,
    timeLeft: ROUND_DURATION_SECONDS,
    status: "idle",
  };
}

export function movePlayer(
  player: Player,
  input: InputState,
  deltaSeconds: number,
  width: number,
  height: number,
): Player {
  const horizontal = Number(input.right) - Number(input.left);
  const vertical = Number(input.down) - Number(input.up);
  const distance = Math.hypot(horizontal, vertical) || 1;

  const nextX = player.x + (horizontal / distance) * player.speed * deltaSeconds;
  const nextY = player.y + (vertical / distance) * player.speed * deltaSeconds;
  const halfSize = player.size / 2;

  return {
    ...player,
    x: clamp(nextX, halfSize, width - halfSize),
    y: clamp(nextY, halfSize, height - halfSize),
  };
}

export function hasCollectedOrb(player: Player, orb: Orb): boolean {
  const playerRadius = player.size / 2;
  const distance = Math.hypot(player.x - orb.x, player.y - orb.y);
  return distance <= playerRadius + orb.radius;
}

export function stepGame(
  state: GameState,
  input: InputState,
  deltaSeconds: number,
  randomFn: () => number = Math.random,
): GameState {
  if (state.status !== "playing") {
    return state;
  }

  const player = movePlayer(state.player, input, deltaSeconds, state.width, state.height);
  const timeLeft = Math.max(0, state.timeLeft - deltaSeconds);

  if (hasCollectedOrb(player, state.orb)) {
    return {
      ...state,
      player,
      orb: spawnOrb(state.width, state.height, state.orb.radius, randomFn),
      score: state.score + 1,
      timeLeft,
      status: timeLeft === 0 ? "finished" : "playing",
    };
  }

  return {
    ...state,
    player,
    timeLeft,
    status: timeLeft === 0 ? "finished" : "playing",
  };
}

