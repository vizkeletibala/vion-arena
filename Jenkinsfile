pipeline {
  agent any

  environment {
    IMAGE_TAG = "${BUILD_NUMBER}"
    REGISTRY = "registry.localhost"
    FRONTEND_IMAGE = "${REGISTRY}/vion-arena-frontend:${IMAGE_TAG}"
    BACKEND_IMAGE = "${REGISTRY}/vion-arena-backend:${IMAGE_TAG}"
    APP_HOST = "arena.vion.test"
    API_HOST = "api.arena.vion.test"
    EDGE_NETWORK = "edge"
    INTERNAL_NETWORK = "internal"
  }

  stages {
    stage('Frontend Checks') {
      steps {
        sh '''
          docker run --rm \
            -v "$PWD":/workspace \
            -w /workspace/frontend \
            node:20-alpine \
            sh -lc "npm ci && npm run lint && npm run test && npm run build"
        '''
      }
    }

    stage('Backend Checks') {
      steps {
        sh '''
          docker run --rm \
            -v "$PWD":/workspace \
            -w /workspace/backend \
            python:3.12-slim \
            sh -lc "pip install --no-cache-dir -r requirements-dev.txt && python -m ruff check app tests && pytest"
        '''
      }
    }

    stage('Build Images') {
      steps {
        sh '''
          docker build -t "$BACKEND_IMAGE" -f backend/Dockerfile backend
          docker build --build-arg VITE_API_BASE_URL="http://$API_HOST" -t "$FRONTEND_IMAGE" -f frontend/Dockerfile frontend
        '''
      }
    }

    stage('Push Images') {
      steps {
        sh '''
          docker push "$BACKEND_IMAGE"
          docker push "$FRONTEND_IMAGE"
        '''
      }
    }

    stage('Deploy') {
      steps {
        sh '''
          BACKEND_IMAGE="$BACKEND_IMAGE" \
          FRONTEND_IMAGE="$FRONTEND_IMAGE" \
          APP_VERSION="$IMAGE_TAG" \
          APP_ALLOWED_ORIGINS="http://$APP_HOST" \
          EDGE_NETWORK="$EDGE_NETWORK" \
          INTERNAL_NETWORK="$INTERNAL_NETWORK" \
          docker compose -f deploy/docker-compose.app.yml up -d
        '''
      }
    }

    stage('Smoke Tests') {
      steps {
        sh '''
          docker run --rm \
            --network host \
            -v "$PWD":/workspace \
            -w /workspace \
            alpine:3.20 \
            sh -lc "apk add --no-cache curl grep >/dev/null && TRAEFIK_URL=http://localhost APP_HOST=$APP_HOST API_HOST=$API_HOST sh scripts/smoke-test.sh"
        '''
      }
    }
  }
}
