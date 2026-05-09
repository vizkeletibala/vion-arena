import type { ScoreRecord } from "./types";

const DEFAULT_API_BASE_URL = "http://localhost:8000";
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || DEFAULT_API_BASE_URL).replace(/\/$/, "");

type ScoresResponse = {
  scores: ScoreRecord[];
};

export async function fetchLeaderboard(): Promise<ScoreRecord[]> {
  const response = await fetch(`${API_BASE_URL}/scores`);
  if (!response.ok) {
    throw new Error(`Failed to fetch scores: ${response.status}`);
  }

  const data = (await response.json()) as ScoresResponse;
  return data.scores;
}

export async function submitScore(name: string, score: number): Promise<ScoreRecord> {
  const response = await fetch(`${API_BASE_URL}/scores`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ name, score }),
  });

  if (!response.ok) {
    throw new Error(`Failed to submit score: ${response.status}`);
  }

  return (await response.json()) as ScoreRecord;
}

