from __future__ import annotations

from datetime import datetime

from pydantic import BaseModel, Field


class HealthResponse(BaseModel):
    status: str = "ok"
    service: str


class VersionResponse(BaseModel):
    version: str


class ScoreSubmission(BaseModel):
    name: str = Field(min_length=1, max_length=20)
    score: int = Field(ge=0, le=9999)


class ScoreRecord(BaseModel):
    name: str
    score: int
    submitted_at: datetime


class ScoresResponse(BaseModel):
    scores: list[ScoreRecord]

