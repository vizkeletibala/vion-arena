import { describe, expect, it } from "vitest";
import { createInitialGameState, movePlayer, stepGame } from "./gameLogic";

describe("game logic", () => {
  it("keeps the player inside the arena bounds", () => {
    const movedPlayer = movePlayer(
      {
        x: 20,
        y: 20,
        size: 26,
        speed: 220,
      },
      {
        up: true,
        down: false,
        left: true,
        right: false,
      },
      1,
      640,
      400,
    );

    expect(movedPlayer.x).toBeGreaterThanOrEqual(13);
    expect(movedPlayer.y).toBeGreaterThanOrEqual(13);
  });

  it("increments the score and respawns the orb on collision", () => {
    const state = {
      ...createInitialGameState(() => 0.5),
      status: "playing" as const,
      player: {
        x: 120,
        y: 120,
        size: 26,
        speed: 220,
      },
      orb: {
        x: 120,
        y: 120,
        radius: 10,
      },
    };

    const nextState = stepGame(
      state,
      { up: false, down: false, left: false, right: false },
      0.16,
      () => 0.25,
    );

    expect(nextState.score).toBe(1);
    expect(nextState.orb.x).not.toBe(120);
    expect(nextState.orb.y).not.toBe(120);
  });

  it("finishes the round when the timer reaches zero", () => {
    const state = {
      ...createInitialGameState(() => 0.5),
      status: "playing" as const,
      timeLeft: 0.05,
    };

    const nextState = stepGame(
      state,
      { up: false, down: false, left: false, right: false },
      0.1,
    );

    expect(nextState.timeLeft).toBe(0);
    expect(nextState.status).toBe("finished");
  });
});
