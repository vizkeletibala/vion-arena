from __future__ import annotations

from prometheus_client import CONTENT_TYPE_LATEST, Counter, Gauge, Histogram, generate_latest

REQUEST_COUNT = Counter(
    "vion_http_requests_total",
    "Total HTTP requests handled by the Vion Arena backend.",
    ["method", "path", "status_code"],
)

REQUEST_LATENCY = Histogram(
    "vion_http_request_duration_seconds",
    "Request latency for the Vion Arena backend.",
    ["method", "path"],
)

SCORES_SUBMITTED = Counter(
    "vion_scores_submitted_total",
    "Number of scores submitted to the leaderboard.",
)

LEADERBOARD_ENTRIES = Gauge(
    "vion_leaderboard_entries",
    "Current number of leaderboard entries stored in memory.",
)


def render_metrics() -> tuple[bytes, str]:
    return generate_latest(), CONTENT_TYPE_LATEST

