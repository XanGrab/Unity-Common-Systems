# Unity Common Reusable Systems ⚙️

## Version 0.1.1

A simple unity package that contains some of my commonly reused systems

- Bootstrapper
- Audio Manager

## Bootstrapper 🧰

A utility function used to load any systems required at Runtime (e.g. `AudioManager`). In the Editor hierarchy, a GameObject named "GlobalSystems" gets added on Runtime Initalized.

# SoundSystem

Use this simple sound system by importing the `SoundSystem` namespace into a file.

## 🎵 Sound

Sound class ineherits from SctiptableObject, and represents a collection of AudioClips for a specific SFX or music track that will be played in game. 

Each sound is automatically loaded into memory of the AudioManager when the attached MonoBehavior enters the scene.

| **Properties** |                                                                                 |
| -------------- | ------------------------------------------------------------------------------- |
| `Clips`     | the collection of AudioClips associated with this Sound |
| `ClipType`    | specifies how clips should be retrieved from this Sound |
| `Mixer` | the output `AudioMixerGroup` this Sound should be played on |
| `Volume` | the volume these Sounds should be played at |
| `Pitch` | the pitch these Sounds should be played at |
| `Loop` | toggles whether or not this Sound should repeat itself |

### Methods

`PlayOneShot` - Triggers the next clip to be played once from the current AudioManager. Useful for SFX.

`Play` - Plays the next clip on this Sound from the current AudioManager, if it is not already playing. Useful for Music.

## 🎹 AudioManager

The AudioManager class a Singleton MonoBehaviour that can be used to play all clips from the currently loaded Sounds in Unity. 

Sounds can be added to the AudioManager manually in the inspector if they should remain persistantly loaded memory for the manager's lifetime, or loaded dynamically at runtime from Sound ScriptableObjects.

| **Properties** |                                                                                 |
| -------------- | ------------------------------------------------------------------------------- |
| `Instance`     | The current instance of the AudioManager class (implements _Singleton_ pattern) |
| `PersistSounds`       | Array of `Sound` objects that can be played at any time during the lifetime of the manager                        |
| `PlayOnAwake`       | When toggled, the manager will attempt to play the first loaded Sound in its internal array as soon as it is added OnAwake() |

### Methods

`PlayOneShot` - Plays a specified Sound as a one shot audio event from the current AudioManager. Useful for SFX.

`Play` - Plays a specified Sound from the current AudioManager, if it is not already playing. Useful for Music.

`PauseToggle` - 

`Stop` - 

`FadeVolume` - 

`FadeTo` - 
