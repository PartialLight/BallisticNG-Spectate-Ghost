# BallisticNG-Spectate-Ghost
### Code Mod that allows ship ghosts to be viewed from the perspective of the ghost ship.

Spectate Ghost is a codemod that allows you to spectate ship ghosts in Time Trial and Speed Lap.

While this mod is active, crossing the finish laser in Time Trial or Speed Lap will instantly attach the player ship to the ghost ship and follow it as closely as possible.

Because of how anti-skip detection and track recovery work, the mod also manually disables anti-skip and track recovery when in these modes to prevent the recovery drone from attaching itself to the player ship when a skip is detected, or when you "enter" a recovery trigger by attaching yourself to a distant ghost.

Ghosts are not perfect representations of the trajectory the ship followed in the original record run. This not only means that viewing the ghost record from the perspective of the ghost can be strange visually, but that there may be inconsistencies in lap times, usually ±0.01 seconds.

Furthermore, ghosts exhibit a strange behavior when they are finishing a lap in Speed Lap or finishing the race in Time Trial, where they rapidly lurch across the finish line in a way that is not representative of the original record run. This introduces much larger inconsistencies of ±0.10 seconds or more, which can cause the original ghost and record to be overwritten with a new, inaccurate ghost and record.

To prevent ghosts and records from being overwritten, the mod will automatically detach the player ship from the ghost ship when you reach the section before the finish laser (always true in Speed Lap, only happens on the final lap in Time Trial) and trigger a respawn, which sets the player ship at a dead stop.

This mod has two in-game mod menu options:

"Camera Rotates With Ghost" determines whether or not the camera should tilt as the ghost ship tilts. Changes to this setting take effect immediately.

"Enable Spectating" determines whether or not the gameplay-altering code that allows for ghosts to be spectated (attaching the player ship to the ghost ship when you cross the finish line, disabling anti-skip and track recovery, forcing ships to stop before the finish laser\) will actually be loaded into Speed Lap and Time Trial. Changes to this setting require a race restart to take effect.

It also has some pre-defined keybinds:

Numpad 9 manually detaches the player ship from the ghost ship when pressed (be aware that crossing the finish line will automatically re-attach the player ship to the ghost ship). 

Numpad 7 manually attaches the player ship to the ghost ship when pressed (be aware that if there is no ghost saved/loaded for the current track+gamemode+speedclass+shipclass configuration, you will become stuck in the floor at the world origin point if you press this).

Numpad 5 manually respawns the player ship at any time when pressed. This is useful if you fall out of the map while detached from the ghost ship, as with track recovery disabled, there is no way to return to the track surface aside from respawning.

Numpad 3 will disable the rendering of the ghost ship when pressed.

Numpad 1 will re-enable the rendering of the ghost ship when pressed.

If you want to change these keybinds, you will need to open the SpectateGhost.cs file and edit lines 98, 102, 107, 112, and 116, enable "Always Recompile" in the Modding>Code Mods>Spectate Ghost menu, and relaunch the game to recompile the mod for your changes to take effect.

[A list of all the Keycodes you can use can be found here](https://docs.unity3d.com/2020.3/Documentation/ScriptReference/KeyCode.html), under the Properties section.
