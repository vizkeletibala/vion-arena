#!/usr/bin/env sh
set -eu

TRAEFIK_URL="${TRAEFIK_URL:-http://localhost}"
APP_HOST="${APP_HOST:-arena.vion.test}"
API_HOST="${API_HOST:-api.arena.vion.test}"

retry_curl() {
  target_url="$1"
  host_header="$2"
  attempt=1

  while [ "$attempt" -le 20 ]; do
    if curl -fsS -H "Host: ${host_header}" "${target_url}" >/dev/null 2>&1; then
      return 0
    fi

    sleep 2
    attempt=$((attempt + 1))
  done

  return 1
}

echo "Checking frontend..."
retry_curl "${TRAEFIK_URL}/" "${APP_HOST}"
curl -fsS -H "Host: ${APP_HOST}" "${TRAEFIK_URL}/" | grep -qi "Vion Arena"

echo "Checking backend health..."
retry_curl "${TRAEFIK_URL}/health" "${API_HOST}"
curl -fsS -H "Host: ${API_HOST}" "${TRAEFIK_URL}/health" | grep -q '"status":"ok"'

echo "Checking backend version..."
curl -fsS -H "Host: ${API_HOST}" "${TRAEFIK_URL}/version" | grep -q '"version"'

echo "Checking metrics..."
curl -fsS -H "Host: ${API_HOST}" "${TRAEFIK_URL}/metrics" | grep -q 'vion_http_requests_total'

echo "Submitting smoke score..."
curl -fsS \
  -X POST \
  -H "Host: ${API_HOST}" \
  -H "Content-Type: application/json" \
  -d '{"name":"Smoke","score":7}' \
  "${TRAEFIK_URL}/scores" | grep -q '"name":"Smoke"'

echo "Checking leaderboard..."
curl -fsS -H "Host: ${API_HOST}" "${TRAEFIK_URL}/scores" | grep -q '"Smoke"'

echo "Smoke test passed."
