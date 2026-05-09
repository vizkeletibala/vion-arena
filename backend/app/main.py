from __future__ import annotations

import logging
import time
from contextlib import asynccontextmanager

from fastapi import FastAPI, Request, Response, status
from fastapi.middleware.cors import CORSMiddleware

from app.config import get_settings
from app.logging_config import configure_logging
from app.metrics import REQUEST_COUNT, REQUEST_LATENCY, SCORES_SUBMITTED, render_metrics
from app.models import HealthResponse, ScoreRecord, ScoresResponse, ScoreSubmission, VersionResponse
from app.store import ScoreStore

configure_logging()
logger = logging.getLogger("vion-arena.backend")


@asynccontextmanager
async def lifespan(_: FastAPI):
    logger.info(
        "backend_startup",
        extra={"event": "backend_startup"},
    )
    yield


app = FastAPI(title="Vion Arena Backend", lifespan=lifespan)
app.state.store = ScoreStore()

settings = get_settings()
app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.allowed_origins or ["*"],
    allow_credentials=False,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.middleware("http")
async def instrument_requests(request: Request, call_next):
    started = time.perf_counter()
    response = await call_next(request)
    duration_seconds = time.perf_counter() - started
    duration_ms = round(duration_seconds * 1000, 2)

    path = request.url.path
    status_code = str(response.status_code)

    REQUEST_COUNT.labels(request.method, path, status_code).inc()
    REQUEST_LATENCY.labels(request.method, path).observe(duration_seconds)

    logger.info(
        "request_complete",
        extra={
            "event": "request_complete",
            "method": request.method,
            "path": path,
            "status_code": response.status_code,
            "duration_ms": duration_ms,
            "client": request.client.host if request.client else "unknown",
        },
    )
    return response


@app.get("/health", response_model=HealthResponse)
def health() -> HealthResponse:
    return HealthResponse(service=get_settings().app_name)


@app.get("/version", response_model=VersionResponse)
def version() -> VersionResponse:
    return VersionResponse(version=get_settings().app_version)


@app.get("/scores", response_model=ScoresResponse)
def list_scores() -> ScoresResponse:
    return ScoresResponse(scores=app.state.store.list_scores())


@app.post("/scores", response_model=ScoreRecord, status_code=status.HTTP_201_CREATED)
def submit_score(payload: ScoreSubmission) -> ScoreRecord:
    SCORES_SUBMITTED.inc()
    record = app.state.store.submit_score(payload.name, payload.score)

    logger.info(
        "score_submitted",
        extra={
            "event": "score_submitted",
            "player_name": record.name,
            "score": record.score,
        },
    )
    return record


@app.get("/metrics")
def metrics() -> Response:
    payload, content_type = render_metrics()
    return Response(content=payload, media_type=content_type)
