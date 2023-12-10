# Unity Common Reusable Systems ⚙️

## Version 0.1.0

A simple unity package that contains some of my commonly reused systems

- Bootstrapper
- Audio Manager

## Bootstrapper 🧰

A utility function used to load any systems required at Runtime (e.g. `AudioManager`). In the Editor hierarchy, a GameObject named "GlobalSystems" gets added on Runtime Initalized.

## Audio Manager 🎹

A utility manager class that utilizes a `Sound` ScriptableObject to play sounds on command from scripts.

| **Properties**           |                                                                                 |
| ---------- | ------------------------------------------------------------------------------- |
| `Instance` | the current instance of the AudioManager class (implements _Singleton_ pattern) |
| `sounds`   | Array of `Sound` objects that can be played at any time                         |

| **Methods**            |                                                                           |
| ---------- | ------------------------------------------------------------------------- |
| `Play`     | plays a specified sound in the current scene if it is not already playing |
| `PlayOnce` | triggers a specified sound to be played in the current scene              |
| `Stop`     |                                                                           |
