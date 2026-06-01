pipeline {
  agent any

  environment {
    IMAGE_TAG = "${BUILD_NUMBER}"
    REGISTRY = "localhost:5000"
    FRONTEND_IMAGE = "${REGISTRY}/vion-arena-frontend:${IMAGE_TAG}"
    BACKEND_IMAGE = "${REGISTRY}/vion-arena-backend:${IMAGE_TAG}"
    VITRIAL_SERVER_IMAGE = "${REGISTRY}/vitrial-server:${IMAGE_TAG}"
    APP_HOST = "arena.vion.test"
    API_HOST = "api.arena.vion.test"
    EDGE_NETWORK = "vion-project_edge"
    INTERNAL_NETWORK = "vion-project_internal"
    ENABLE_UNITY_SERVER_BUILD = "false"
    UNITY_SERVER_BUILD_DIR = "Builds/LinuxServer"
    UNITY_SERVER_BINARY = "VitrialServer.x86_64"
  }

  stages {
    stage('Vitrial Headless Scaffold Checks') {
      steps {
        sh '''
          docker run --rm \
            --volumes-from "$HOSTNAME" \
            -w "$PWD" \
            python:3.12-slim \
            sh scripts/validate-vitrial-headless.sh
        '''
      }
    }

    stage('Frontend Checks') {
      steps {
        sh '''
          docker run --rm \
            --volumes-from "$HOSTNAME" \
            -w "$PWD/frontend" \
            node:20-alpine \
            sh -lc "npm ci && npm run lint && npm run test && npm run build"
        '''
      }
    }

    stage('Backend Checks') {
      steps {
        sh '''
          docker run --rm \
            --volumes-from "$HOSTNAME" \
            -w "$PWD/backend" \
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

    stage('Build Future Unity Server Image') {
      when {
        expression { env.ENABLE_UNITY_SERVER_BUILD == 'true' }
      }
      steps {
        sh '''
          test -f "$UNITY_SERVER_BUILD_DIR/$UNITY_SERVER_BINARY"
          docker build \
            --build-arg SERVER_BUILD_DIR="$UNITY_SERVER_BUILD_DIR" \
            --build-arg SERVER_BINARY="$UNITY_SERVER_BINARY" \
            -t "$VITRIAL_SERVER_IMAGE" \
            -f deploy/Dockerfile.vitrial-server .
          docker push "$VITRIAL_SERVER_IMAGE"
        '''
      }
    }

    stage('Deploy Future Unity Server') {
      when {
        expression { env.ENABLE_UNITY_SERVER_BUILD == 'true' }
      }
      steps {
        sh '''
          VITRIAL_SERVER_IMAGE="$VITRIAL_SERVER_IMAGE" \
          INTERNAL_NETWORK="$INTERNAL_NETWORK" \
          docker compose -f deploy/docker-compose.vitrial-server.yml --profile vitrial-server up -d
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
          ARENA_HOST="$APP_HOST" \
          ARENA_API_HOST="$API_HOST" \
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
            --volumes-from "$HOSTNAME" \
            -w "$PWD" \
            alpine:3.20 \
            sh -lc "apk add --no-cache curl grep >/dev/null && TRAEFIK_URL=http://localhost APP_HOST=$APP_HOST API_HOST=$API_HOST sh scripts/smoke-test.sh"
        '''
      }
    }
  }
}
