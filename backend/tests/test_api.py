from __future__ import annotations

from fastapi.testclient import TestClient

from app.config import get_settings
from app.main import app
from app.store import ScoreStore


def build_client() -> TestClient:
    app.state.store = ScoreStore()
    get_settings.cache_clear()
    return TestClient(app)


def test_health_endpoint() -> None:
    with build_client() as client:
        response = client.get("/health")

    assert response.status_code == 200
    assert response.json() == {"status": "ok", "service": "vion-arena-backend"}


def test_version_endpoint(monkeypatch) -> None:
    monkeypatch.setenv("APP_VERSION", "1.2.3-test")

    with build_client() as client:
        response = client.get("/version")

    assert response.status_code == 200
    assert response.json() == {"version": "1.2.3-test"}


def test_scores_endpoint_orders_high_scores() -> None:
    with build_client() as client:
        empty_response = client.get("/scores")
        assert empty_response.status_code == 200
        assert empty_response.json() == {"scores": []}

        client.post("/scores", json={"name": "Mira", "score": 4})
        client.post("/scores", json={"name": "Ivo", "score": 9})

        response = client.get("/scores")

    assert response.status_code == 200
    assert [entry["name"] for entry in response.json()["scores"]] == ["Ivo", "Mira"]


def test_metrics_endpoint_exposes_prometheus_metrics() -> None:
    with build_client() as client:
        client.get("/health")
        response = client.get("/metrics")

    assert response.status_code == 200
    assert "vion_http_requests_total" in response.text
    assert "vion_http_request_duration_seconds" in response.text
