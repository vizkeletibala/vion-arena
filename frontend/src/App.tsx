import { useEffect, useRef, useState, type FormEvent } from "react";
import { fetchLeaderboard, submitScore } from "./game/api";
import {
  EMPTY_INPUT,
  createInitialGameState,
  stepGame,
} from "./game/gameLogic";
import { GAME_HEIGHT, GAME_WIDTH, ROUND_DURATION_SECONDS } from "./game/constants";
import type { GameState, InputState, ScoreRecord } from "./game/types";

function drawArena(context: CanvasRenderingContext2D, state: GameState): void {
  context.clearRect(0, 0, state.width, state.height);

  const gradient = context.createLinearGradient(0, 0, state.width, state.height);
  gradient.addColorStop(0, "#07111f");
  gradient.addColorStop(1, "#0f2b46");
  context.fillStyle = gradient;
  context.fillRect(0, 0, state.width, state.height);

  context.strokeStyle = "rgba(255, 255, 255, 0.08)";
  for (let x = 0; x <= state.width; x += 32) {
    context.beginPath();
    context.moveTo(x, 0);
    context.lineTo(x, state.height);
    context.stroke();
  }
  for (let y = 0; y <= state.height; y += 32) {
    context.beginPath();
    context.moveTo(0, y);
    context.lineTo(state.width, y);
    context.stroke();
  }

  context.fillStyle = "#f9d423";
  context.beginPath();
  context.arc(state.orb.x, state.orb.y, state.orb.radius, 0, Math.PI * 2);
  context.fill();

  context.fillStyle = "#6ae3ff";
  context.fillRect(
    state.player.x - state.player.size / 2,
    state.player.y - state.player.size / 2,
    state.player.size,
    state.player.size,
  );

  context.fillStyle = "#ffffff";
  context.font = "600 18px monospace";
  context.fillText(`Score: ${state.score}`, 16, 28);
  context.fillText(`Time: ${Math.ceil(state.timeLeft)}`, 16, 54);

  if (state.status !== "playing") {
    context.fillStyle = "rgba(7, 17, 31, 0.72)";
    context.fillRect(0, 0, state.width, state.height);
    context.fillStyle = "#ffffff";
    context.textAlign = "center";
    context.font = "700 28px monospace";
    const message =
      state.status === "finished" ? "Round Over" : "Press Start To Play";
    context.fillText(message, state.width / 2, state.height / 2 - 8);
    context.font = "500 16px monospace";
    context.fillText("Use WASD or arrow keys to collect as many orbs as you can.", state.width / 2, state.height / 2 + 24);
    context.textAlign = "start";
  }
}

export default function App() {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const animationFrameRef = useRef<number | null>(null);
  const lastFrameRef = useRef<number | null>(null);
  const inputRef = useRef<InputState>({ ...EMPTY_INPUT });

  const [gameState, setGameState] = useState(() => createInitialGameState());
  const [leaderboard, setLeaderboard] = useState<ScoreRecord[]>([]);
  const [playerName, setPlayerName] = useState("Player");
  const [loadingScores, setLoadingScores] = useState(true);
  const [submitState, setSubmitState] = useState<"idle" | "submitting" | "submitted" | "error">("idle");
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    const loadScores = async () => {
      setLoadingScores(true);
      setErrorMessage("");

      try {
        const scores = await fetchLeaderboard();
        setLeaderboard(scores);
      } catch (error) {
        setErrorMessage(error instanceof Error ? error.message : "Could not load leaderboard.");
      } finally {
        setLoadingScores(false);
      }
    };

    void loadScores();
  }, []);

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "ArrowUp" || event.key === "w" || event.key === "W") {
        inputRef.current.up = true;
      }
      if (event.key === "ArrowDown" || event.key === "s" || event.key === "S") {
        inputRef.current.down = true;
      }
      if (event.key === "ArrowLeft" || event.key === "a" || event.key === "A") {
        inputRef.current.left = true;
      }
      if (event.key === "ArrowRight" || event.key === "d" || event.key === "D") {
        inputRef.current.right = true;
      }
    };

    const handleKeyUp = (event: KeyboardEvent) => {
      if (event.key === "ArrowUp" || event.key === "w" || event.key === "W") {
        inputRef.current.up = false;
      }
      if (event.key === "ArrowDown" || event.key === "s" || event.key === "S") {
        inputRef.current.down = false;
      }
      if (event.key === "ArrowLeft" || event.key === "a" || event.key === "A") {
        inputRef.current.left = false;
      }
      if (event.key === "ArrowRight" || event.key === "d" || event.key === "D") {
        inputRef.current.right = false;
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    window.addEventListener("keyup", handleKeyUp);

    return () => {
      window.removeEventListener("keydown", handleKeyDown);
      window.removeEventListener("keyup", handleKeyUp);
    };
  }, []);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) {
      return;
    }

    const context = canvas.getContext("2d");
    if (!context) {
      return;
    }

    drawArena(context, gameState);
  }, [gameState]);

  useEffect(() => {
    if (gameState.status !== "playing") {
      if (animationFrameRef.current !== null) {
        cancelAnimationFrame(animationFrameRef.current);
        animationFrameRef.current = null;
      }
      lastFrameRef.current = null;
      return;
    }

    const tick = (timestamp: number) => {
      if (lastFrameRef.current === null) {
        lastFrameRef.current = timestamp;
      }

      const deltaSeconds = Math.min((timestamp - lastFrameRef.current) / 1000, 0.05);
      lastFrameRef.current = timestamp;

      setGameState((currentState) => stepGame(currentState, inputRef.current, deltaSeconds));
      animationFrameRef.current = window.requestAnimationFrame(tick);
    };

    animationFrameRef.current = window.requestAnimationFrame(tick);

    return () => {
      if (animationFrameRef.current !== null) {
        cancelAnimationFrame(animationFrameRef.current);
      }
      animationFrameRef.current = null;
      lastFrameRef.current = null;
    };
  }, [gameState.status]);

  const startRound = () => {
    inputRef.current = { ...EMPTY_INPUT };
    setSubmitState("idle");
    setErrorMessage("");
    setGameState({
      ...createInitialGameState(),
      status: "playing",
      timeLeft: ROUND_DURATION_SECONDS,
    });
  };

  const handleSubmitScore = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setSubmitState("submitting");
    setErrorMessage("");

    try {
      await submitScore(playerName.trim() || "Player", gameState.score);
      const scores = await fetchLeaderboard();
      setLeaderboard(scores);
      setSubmitState("submitted");
    } catch (error) {
      setSubmitState("error");
      setErrorMessage(error instanceof Error ? error.message : "Score submission failed.");
    }
  };

  return (
    <main className="layout">
      <section className="hero">
        <div className="hero-copy">
          <p className="eyebrow">Vion Arena</p>
          <h1>Collect glowing orbs before the timer runs out.</h1>
          <p className="lede">
            This first version stays intentionally small: one arena, one player, one leaderboard,
            and enough metrics and logs to exercise the platform around it.
          </p>
          <div className="actions">
            <button className="primary-button" onClick={startRound} type="button">
              {gameState.status === "finished" ? "Play Again" : "Start Round"}
            </button>
            <span className="help-text">Move with WASD or arrow keys.</span>
          </div>
        </div>

        <div className="game-shell">
          <canvas
            aria-label="Vion Arena game canvas"
            className="arena-canvas"
            height={GAME_HEIGHT}
            ref={canvasRef}
            width={GAME_WIDTH}
          />
        </div>
      </section>

      <section className="panel-grid">
        <article className="panel">
          <div className="panel-heading">
            <h2>Submit Score</h2>
            <span>{gameState.score} points</span>
          </div>
          <form className="score-form" onSubmit={handleSubmitScore}>
            <label htmlFor="playerName">Player name</label>
            <input
              id="playerName"
              maxLength={20}
              onChange={(event) => setPlayerName(event.target.value)}
              value={playerName}
            />
            <button
              className="secondary-button"
              disabled={gameState.status !== "finished" || submitState === "submitting"}
              type="submit"
            >
              {submitState === "submitting" ? "Submitting..." : "Send To Leaderboard"}
            </button>
          </form>
          <p className="panel-note">
            Finish a {ROUND_DURATION_SECONDS}-second round, then submit the result to the FastAPI backend.
          </p>
          {submitState === "submitted" ? <p className="success-text">Score submitted.</p> : null}
          {errorMessage ? <p className="error-text">{errorMessage}</p> : null}
        </article>

        <article className="panel">
          <div className="panel-heading">
            <h2>Leaderboard</h2>
            <span>Top collectors</span>
          </div>
          {loadingScores ? (
            <p className="panel-note">Loading leaderboard...</p>
          ) : (
            <ol className="leaderboard">
              {leaderboard.length === 0 ? (
                <li className="empty-state">No scores yet. Be the first.</li>
              ) : (
                leaderboard.map((entry) => (
                  <li key={`${entry.name}-${entry.submitted_at}`}>
                    <span>{entry.name}</span>
                    <strong>{entry.score}</strong>
                  </li>
                ))
              )}
            </ol>
          )}
        </article>
      </section>
    </main>
  );
}
