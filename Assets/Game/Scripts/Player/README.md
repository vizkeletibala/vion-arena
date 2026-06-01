# Assets/Game/Scripts/Player

Greybox Vitrial playable controller for `Assets/Game/Scenes/PrototypeArena.unity`.

## Windows playtest controls

- WASD: move on the ground plane.
- Mouse: look left/right and pitch the camera pivot.
- Left Shift: sprint while held.
- Space: jump.
- Esc / Alt+Tab: release focus with standard Windows editor controls if needed.

The controller uses Unity's built-in legacy input axes (`Horizontal`, `Vertical`, `Mouse X`, `Mouse Y`, `Jump`) plus direct key reads so it works in a fresh 2022.3 editor without Input System package setup.

## Component boundaries

- `PlayerInputReader` is the only component that reads keyboard/mouse state.
- `PlayerMotor` owns `CharacterController` movement, sprint speed, jumping, and gravity.
- `PlayerLookController` rotates the player yaw and `CameraPivot` pitch.
- `PlayerRigAnchor` exposes stable scene hooks: `CameraPivot`, `WeaponSocket`, `InputReader`, `Motor`, and `LookController`.
- The `WeaponSocket` mounts the Starter Rifle child; fire with Left Mouse Button and reload with `reload: R`.

## Downstream input contract

Weapon, enemy, loot, and UI lanes should depend on `PlayerInputReader` and these action meanings instead of adding duplicate raw input checks:

- fire: Left Mouse Button
- reload: R
- interact: E
- inventory: Tab

The current prefab also exposes `WeaponSocket` for weapon attachment and keeps movement code free of weapon/enemy/UI concerns.
