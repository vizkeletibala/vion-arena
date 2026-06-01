#!/usr/bin/env sh
set -eu

ROOT_DIR="${ROOT_DIR:-$(CDPATH= cd -- "$(dirname -- "$0")/.." && pwd)}"
cd "$ROOT_DIR"

python3 -m json.tool Packages/manifest.json >/dev/null
python3 -m json.tool Assets/Game/Vitrial.Game.asmdef >/dev/null

required_paths='Assets/Game
Assets/Game/Scenes
Assets/Game/Scripts/Networking
Assets/Game/Scripts/GameState
Assets/Game/Scripts/Player
Assets/Game/Scripts/Weapons
Assets/Game/Scripts/Enemies
Assets/Game/Scripts/Loot
Assets/Game/Scripts/Inventory
Assets/Game/Scripts/UI
Assets/Game/Prefabs/Player
Assets/Game/Prefabs/Weapons
Assets/Game/Prefabs/Enemies
Assets/Game/ScriptableObjects/Weapons
Assets/Game/ScriptableObjects/Enemies
ProjectSettings/ProjectVersion.txt
Packages/manifest.json'

printf '%s\n' "$required_paths" | while IFS= read -r path; do
  [ -e "$path" ] || {
    echo "missing required Vitrial scaffold path: $path" >&2
    exit 1
  }
done

python3 - <<'PY'
from pathlib import Path
for path in sorted(Path('Assets/Game/Scripts').rglob('*.cs')):
    text = path.read_text(encoding='utf-8')
    if text.count('{') != text.count('}'):
        raise SystemExit(f'unbalanced braces in {path}')
print('validated Vitrial scaffold JSON, required paths, and C# brace balance')
PY
