# Vitrial Unity CI/CD Strategy

This recommendation is for Andrew's current Vitrial lane in this repository. It is grounded in the present split between the Vion web stack, the Unity source scaffold under `Assets/Game/`, and the EC2/Jenkins environment already represented by `Jenkinsfile` and `docs/vitrial-automation.md`.

## Executive recommendation

### Preferred near-term path

Keep the existing EC2/Jenkins lane as a static/headless validation and packaging lane only:

- run `scripts/validate-vitrial-headless.sh`;
- validate JSON syntax for `Packages/manifest.json` and `Assets/Game/Vitrial.Game.asmdef`;
- run repository hygiene checks such as `git diff --check`;
- run non-Unity shell/Python checks and existing frontend/backend CI;
- package/deploy only artifacts that already exist and are explicitly enabled.

Do not present the EC2 host or current Jenkins controller as a graphical Unity workstation, Unity Editor import gate, real Unity C# compile gate, playmode-test runner, or Linux dedicated-server build generator. The current EC2/Jenkins lane can catch broken files and missing scaffold paths, but it cannot honestly claim Unity build success without a Unity-capable runner and a valid Unity license.

### Preferred medium-term automated path

Add a separate Windows Unity-capable build lane once Andrew wants automated Unity compile/playmode confidence. The best first automated option is either:

1. GitHub Actions on a Windows runner using GameCI/unity-builder plus Unity license secrets, if Vitrial is comfortable putting Unity CI secrets in GitHub and wants a mostly managed runner; or
2. a Jenkins Windows agent with Unity 2022.3 LTS installed/licensed, if Andrew wants the Unity lane to stay inside the existing Jenkins dashboard and branch policy.

For the smallest operational burden, start with GitHub Actions Windows + GameCI. For tighter integration with the existing Vion/Jenkins deployment story, choose a Jenkins Windows agent. In both cases, keep EC2/Jenkins static checks as the fast first gate and make the Unity-capable lane a separate, clearly named required check before merging Unity-impacting changes.

## What EC2/Jenkins can validate now

The current EC2/Jenkins environment is useful, but its boundary must stay explicit.

Current valid checks:

- `scripts/validate-vitrial-headless.sh`
  - `python3 -m json.tool Packages/manifest.json`
  - `python3 -m json.tool Assets/Game/Vitrial.Game.asmdef`
  - required Unity source/scaffold paths under `Assets/Game/`, `Packages/`, and `ProjectSettings/`
  - simple C# brace-balance sanity checking under `Assets/Game/Scripts/`
- existing browser stack checks:
  - frontend `npm ci`, lint, tests, build
  - backend ruff and pytest
- shell validations such as Docker Compose config checks for existing compose files;
- repository hygiene:
  - generated/cache directories absent from git;
  - no whitespace-only diff problems via `git diff --check`;
  - no accidental `.csproj`, `.sln`, `Library/`, `Temp/`, `Obj/`, `Build/`, `Builds/`, `Logs/`, or `UserSettings/` additions.

Current invalid claims:

- Unity Editor project import passed on EC2;
- Unity C# compilation passed through Unity assemblies;
- Unity editmode/playmode tests passed;
- `Assets/Game/Scenes/PrototypeArena.unity` was opened and validated in the Editor;
- a Linux dedicated-server build was generated;
- EC2 is a viable graphical Unity workstation for Andrew's manual validation.

The `ENABLE_UNITY_SERVER_BUILD=false` default in `Jenkinsfile` is the right posture. Enable server image packaging only after a real Unity lane has produced `Builds/LinuxServer/VitrialServer.x86_64` or an equivalent archived artifact that Jenkins can retrieve.

## Unity-capable validation options

### Option A: Manual Windows desktop validation lane

Use Andrew's Windows desktop with Unity Hub and Unity Editor `2022.3.55f1` or another agreed 2022.3 LTS patch.

What it validates:

- project opens in Unity;
- `Assets/Game/Scenes/PrototypeArena.unity` imports;
- Unity C# compile errors surface in the Editor;
- manual playtest loop from `README.md` works;
- intentional `.unity`, `.prefab`, `.asset`, `.meta`, and `ProjectSettings/` changes are saved and reviewed.

Prerequisites:

- Unity Hub;
- Unity 2022.3 LTS matching or intentionally updating `ProjectSettings/ProjectVersion.txt`;
- a Unity account/license suitable for Andrew's usage;
- local Git access to the repo.

Risks/tradeoffs:

- lowest setup cost;
- no new infrastructure;
- still subjective and manual;
- hard to enforce as a merge gate;
- catches real Unity issues that EC2 cannot catch, but only when Andrew remembers to run it.

Recommended use:

- keep this now as the required human sanity check for Unity-impacting changes;
- record the exact Unity version and playtest notes in review comments when Unity assets/scripts change.

### Option B: GitHub Actions Windows runner with GameCI/unity-builder

Use a Windows GitHub Actions workflow to run Unity activation/import/build/test steps with GameCI tooling and repository secrets.

What it can validate:

- Unity project import on a Windows runner;
- C# compilation through Unity;
- editmode and playmode tests if Vitrial adds them;
- optional Windows client or Linux dedicated-server build artifacts;
- artifacts uploaded to GitHub Actions for Jenkins or manual download.

Prerequisites:

- GitHub Actions enabled for the repository;
- Unity license credentials/secrets appropriate for GameCI, commonly one of:
  - `UNITY_LICENSE` for a generated `.ulf` license file; or
  - Unity email/password/serial style secrets if Andrew chooses a flow that supports them;
- explicit decision on whether builds are Windows client, Linux dedicated server, or both;
- workflow branch/path filters so Unity jobs run when `Assets/`, `Packages/`, `ProjectSettings/`, or Unity test files change;
- artifact retention policy for build outputs and test results.

Risks/tradeoffs:

- easiest managed automated lane;
- clearer GitHub branch protection integration;
- Windows runner minutes can be slower/costly;
- Unity licensing secrets live in GitHub;
- GameCI image/action behavior must match Unity 2022.3 LTS and license terms;
- Jenkins will need either to stay separate or consume GitHub artifacts/statuses.

Recommended use:

- preferred medium-term automated path if Andrew wants fast adoption and GitHub branch protection;
- name the required check something explicit like `unity-windows-import-compile` so nobody confuses it with the EC2 headless scaffold check.

### Option C: Jenkins Windows agent with Unity installed/licensed

Attach a Windows node/agent to the existing Jenkins controller and install Unity Hub/Editor on that node.

What it can validate:

- Unity import/compile through a real Editor installation;
- editmode/playmode tests;
- Windows client builds;
- Linux dedicated-server builds if the Unity installation includes the Linux Dedicated Server build support module;
- Jenkins-native artifact archival and downstream packaging/deploy coordination.

Prerequisites:

- Windows machine or VM reachable by Jenkins;
- Jenkins agent service account;
- Unity Hub/Unity Editor `2022.3.55f1` or agreed 2022.3 LTS patch;
- Unity license activation under the Jenkins agent account;
- secure credential storage in Jenkins for any Unity activation flow;
- workspace cleanup policy for Unity-generated `Library/`, `Temp/`, `Obj/`, `Logs/`, and build outputs;
- Jenkinsfile restructuring so Unity stages run only on the Windows Unity label, while existing EC2 stages remain on the current Docker/Jenkins lane.

Risks/tradeoffs:

- best fit if Andrew wants one Jenkins dashboard and one promotion path;
- more operational maintenance than GitHub Actions;
- requires a Windows host lifecycle, agent reliability, Unity updates, license reactivation handling, and cache cleanup;
- branch policy must distinguish EC2 static checks from Windows Unity checks.

Recommended use:

- choose this if Jenkins is already Andrew's source of truth for CI/CD and if the team can maintain a Windows agent;
- keep the Unity node label explicit, for example `unity-windows`, to avoid accidental execution on EC2.

### Option D: Dedicated Unity build host

Provision a persistent build machine specifically for Unity builds, separate from both Andrew's desktop and the current EC2/Jenkins controller.

What it can validate:

- the same Unity import/compile/test/build capabilities as the Jenkins Windows agent;
- potentially faster builds through persistent caches;
- stable dedicated-server artifact production for the future `deploy/Dockerfile.vitrial-server` path.

Prerequisites:

- dedicated Windows or Unity-supported build host;
- Unity installation and license activation;
- secure CI runner/agent integration, either Jenkins or GitHub self-hosted runner;
- backup/update policy;
- cache retention policy;
- artifact publishing path back to Jenkins/GitHub/registry.

Risks/tradeoffs:

- most control and best long-run performance;
- highest operational ownership;
- host drift and Unity license state become Andrew's responsibility;
- not justified until Unity builds are frequent enough to amortize the maintenance.

Recommended use:

- defer until Vitrial has frequent automated builds, stable Unity test suites, or dedicated server artifacts that need predictable turnaround.

## Secrets, licenses, and artifacts

Unity-capable lanes need secrets that the current EC2 static lane does not need:

- Unity license material for the chosen runner:
  - `.ulf` license file content or GameCI-supported activation credentials;
  - Unity serial if the license model requires it;
  - Unity account email/password only if Andrew accepts that flow and stores it in the CI secret manager;
- GitHub or Jenkins credential entries scoped to the Unity job, not exposed to frontend/backend jobs;
- artifact upload credentials only if builds are published outside the CI system;
- optional code-signing credentials later, if Windows client artifacts become distributable.

Do not commit these secrets to git. Do not write them into `ProjectSettings/`, Editor logs, shell traces, or generated artifacts. Prefer a dedicated secret namespace such as GitHub Actions `UNITY_LICENSE` or Jenkins credentials scoped to a `unity-windows` folder/job.

## Cache and build directory policy

Keep Unity caches and build outputs out of git:

- `Library/`
- `Temp/`
- `Obj/`
- `Build/`
- `Builds/`
- `Logs/`
- `UserSettings/`
- generated `.csproj` and `.sln` files

Reasons:

- `Library/` and `Temp/` are machine-specific and can be huge;
- Unity regenerates project files and caches differently by Editor patch, OS, installed modules, and asset import state;
- committing generated build outputs makes diffs noisy and branch merges brittle;
- build artifacts need retention, provenance, and expiration, which CI artifact storage handles better than git.

Use CI caches for `Library/` only if the runner setup supports stable cache keys based on Unity version plus `Assets/`, `Packages/`, and `ProjectSettings/` content. Cache misses should slow the job, not change correctness.

## Branch policy and validation naming

Keep branch protection understandable by naming checks after what they actually prove.

Suggested checks:

- `ec2-headless-vitrial-scaffold`: fast current Jenkins check; proves JSON, required paths, shell checks, and source hygiene only.
- `web-frontend-ci`: existing frontend lint/test/build.
- `web-backend-ci`: existing backend ruff/pytest.
- `unity-windows-import-compile`: future Unity-capable lane; proves Unity can import and compile the project.
- `unity-playmode-tests`: future optional check once playmode tests exist.
- `unity-linux-server-build`: future optional check once a dedicated-server build script/artifact exists.

Recommended policy:

- Require EC2 static/headless checks for every branch now.
- Require Andrew's manual Windows Unity validation before merging Unity-impacting changes until an automated Unity-capable lane exists.
- Once Option B or C is live, require the Unity import/compile check for changes under `Assets/`, `Packages/`, `ProjectSettings/`, Unity test folders, and build scripts.
- Do not require server build packaging until the Unity lane reliably produces `Builds/LinuxServer/VitrialServer.x86_64` or a CI artifact with the same contents.

## Recommended staged rollout

1. Keep the current Jenkins lane unchanged for EC2 static/headless validation.
2. Add review checklist language for Unity-impacting PRs: Andrew validates on Windows Unity Editor and notes Unity version plus scene/playtest outcome.
3. Choose the automated Unity lane:
   - GitHub Actions Windows + GameCI if speed of adoption and branch protection are most important;
   - Jenkins Windows agent if one Jenkins dashboard and artifact handoff to the EC2 deployment lane are more important.
4. Add a minimal Unity import/compile job first. Do not start with production build/deploy complexity.
5. Add editmode/playmode tests when Vitrial has real tests worth enforcing.
6. Add Linux dedicated-server build generation only after the compile/test lane is stable and the server target is ready.
7. Wire Jenkins server packaging only after a trusted Unity lane publishes the server artifact.

## Bottom line for Andrew

Use EC2/Jenkins now for what it is good at: fast static checks, repository hygiene, current web CI, Docker packaging, and deployment orchestration. Do not use or describe it as a Unity workstation or a real Unity compile gate.

For real Unity confidence, keep manual Windows Editor validation immediately, then graduate to an automated Windows Unity lane. Prefer GitHub Actions Windows + GameCI for the quickest medium-term automation, or a Jenkins Windows agent if Andrew wants all Unity artifacts and promotion decisions to remain inside Jenkins.
