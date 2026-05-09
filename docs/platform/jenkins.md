# Jenkins

Jenkins is the CI/CD orchestrator in this stack.

## Current Service Definition

- image: `jenkins/jenkins:lts-jdk17`
- route: `jenkins.vion.test`
- direct host port: `8081`
- persistent volume: `jenkins-home`
- runs as `root`
- mounts `/var/run/docker.sock`

The Docker socket mount lets Jenkins build images and run Docker commands on the host daemon.

## Why It Exists In This Stack

Jenkins is positioned to:

- build application images
- push images to the local registry
- trigger deployment workflows
- host project-specific automation for teams that want a visible CI server

## Bootstrap Notes

The setup wizard is enabled through:

```text
JAVA_OPTS=-Djenkins.install.runSetupWizard=true
```

A new environment should expect first-run manual setup unless that is later automated.

## Typical Workflow

For a game project, Jenkins can:

1. clone the repo
2. build backend or tools images
3. run tests
4. push tagged images to the registry
5. deploy updated services with Docker Compose or another target system

## Example Responsibilities In A Game Repo

- build the game backend API container
- build admin or launcher services
- package asset-processing workers
- publish server tools to the local registry
- run smoke tests against staging services

## Example Docker-Centric Pipeline Shape

```groovy
pipeline {
  agent any

  stages {
    stage('Build') {
      steps {
        sh 'docker build -t registry.game.test/game-api:${BUILD_NUMBER} .'
      }
    }

    stage('Push') {
      steps {
        sh 'docker push registry.game.test/game-api:${BUILD_NUMBER}'
      }
    }
  }
}
```

## Operational Notes

- Jenkins state is stored in the `jenkins-home` volume.
- Because it has Docker socket access, Jenkins is effectively privileged on the host.
- There is no reverse-proxy auth layer configured here; access control must be handled by Jenkins itself or by tightening Traefik exposure.

## If Reused In Another Repo

Keep Jenkins if:

- the team already knows it
- you want UI-based jobs and pipelines
- you need flexible Docker-heavy automation on a local server

Consider replacing it if:

- the new repo already uses GitHub Actions, GitLab CI, or another hosted system
- you want less maintenance
- you want stronger default security boundaries
