from __future__ import annotations

from datetime import UTC, datetime
from threading import Lock

from app.metrics import LEADERBOARD_ENTRIES
from app.models import ScoreRecord


class ScoreStore:
    def __init__(self, limit: int = 10) -> None:
        self._limit = limit
        self._scores: list[ScoreRecord] = []
        self._lock = Lock()

    def list_scores(self) -> list[ScoreRecord]:
        with self._lock:
            return list(self._scores)

    def submit_score(self, name: str, score: int) -> ScoreRecord:
        record = ScoreRecord(
            name=name.strip() or "Player",
            score=score,
            submitted_at=datetime.now(UTC),
        )

        with self._lock:
            self._scores.append(record)
            self._scores.sort(key=lambda entry: (-entry.score, entry.submitted_at))
            self._scores = self._scores[: self._limit]
            LEADERBOARD_ENTRIES.set(len(self._scores))
            return record

    def reset(self) -> None:
        with self._lock:
            self._scores = []
            LEADERBOARD_ENTRIES.set(0)

