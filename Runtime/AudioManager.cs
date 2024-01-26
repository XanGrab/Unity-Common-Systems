using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem {
    /// <summary>
    /// Class <c>AudioManager</c> is used to play music and SFX from scenes in Unity. A <see cref="Sound"/> can be added to the AudioManager manually in the inspector if they should remain persistantly loaded in memory, or loaded at OnEnable during runtime from Sound ScriptableObjects.
    /// </summary>
    [RequireComponent(typeof(AudioListener), typeof(AudioSource))]
    public class AudioManager : MonoBehaviour {
        private static AudioManager _instance;
        /// <summary> 
        /// The public instance of the AudioManager in the current scene
        /// </summary> 
        public static AudioManager Instance => _instance;

        private static AudioSource speaker;

        /// <summary> 
        /// Sounds that should remain persistent across the runtime of the game. These will be loaded in OnAwake effectivley similar to enabling preloadAudioData
        /// </summary> 
        [SerializeField] private Sound[] persistSounds;
        /// <summary> 
        /// Sounds that should remain persistent across the runtime of the game. These will be loaded in OnAwake effectivley similar to enabling preloadAudioData
        /// </summary> 
        [SerializeField] private bool playOnAwake = false;

        /// <summary> 
        /// An internal array of all playable sounds in the <c>AudioManager</c>
        /// </summary>
        private static List<Sound> tracks = new List<Sound>();

        /// <summary> 
		/// Get the current timestamp from an <c>AudioSource</c> of the <c>AudioManager</c>
		/// </summary>
        public static float GetTimestamp => speaker.time;

        public void Awake() {
            if (_instance != null && _instance != this) {
                Debug.LogError("AudioManager is a Singleton and should not be added to scene more than once.");
                Destroy(this);
                return;
            } else {
                _instance = this;
            }

            speaker = gameObject.GetComponent<AudioSource>();

            if (playOnAwake && tracks.Count > 0) Play(tracks[0].name);
        }

        #region HelperMethods

        private static Sound GetSound(string name) {
            return tracks.Find(sound => sound.name == name);
        }

        private static AudioSource ReadySource(Sound sound, AudioSource src = null) {
            if (src == null) src = speaker;

            src.clip = sound.GetClip();
            src.volume = sound.Volume;
            src.pitch = sound.Pitch;
            src.loop = sound.Loop;
            src.outputAudioMixerGroup = sound.Mixer;
            return src;
        }

        /// <summary> 
        /// Load a collection of sounds into the <c>AudioManager</c> to be played in the current scene
        /// </summary>
        /// <param name="toLoad">the collection sounmds to be loaded</param>
        public static void LoadSounds(IEnumerable<Sound> toLoad) {
            foreach (Sound sound in toLoad) tracks.Add(sound);
        }

        /// <summary> 
        /// Remove a collection of sounds from the <c>AudioManager</c> no londer needed in the current scene
        /// </summary>
        /// <param name="toUnload">the collection sounmds to be removed</param>
        public static void UnloadSounds(IEnumerable<Sound> toUnload) {
            foreach (Sound sound in toUnload) tracks.Remove(sound);
        }

        #endregion // HelperMethods

        #region BasicFunctionality

        /// <summary> 
        /// Play a one shot sound effect from an AudioSource
        /// </summary>
        /// <param name="name">the name of the sound to be played <c>AudioManager</c></param>
        public static void PlayOneShot(string name) {
            Sound sound = GetSound(name);
            PlayOneShot(sound);
        }

        /// <summary> 
        /// Play a one shot sound effect from an AudioSource
        /// </summary>
        /// <param name="sound">the sound to be played </param>
        public static void PlayOneShot(Sound sound) {
            speaker.PlayOneShot(sound.GetClip());
        }

        /// <summary> 
        /// Begin playing a sound from the main source of the <c>AudioManager</c>
        /// </summary>
        /// <param name="name">the name of the sound to be played </param>
        /// <param name="startTime"> [optional] starting timestamp of the clip </param>
        /// <param name="fadeDuration"> [optional] specifify a duration for which the sound should fade-in </param>
        public static void Play(string name, float startTime = 0f, float fadeDuration = 0f) {
            Sound sound = GetSound(name);
            Play(sound, startTime, fadeDuration);
        }

        /// <summary> 
        /// Begin playing a sound from the main source of the <c>AudioManager</c>
        /// </summary>
        /// <param name="sound">the sound to be played </param>
        /// <param name="startTime"> [optional] the starting timestamp of the clip </param>
        /// <param name="fadeDuration"> [optional] specifify a duration for which the sound should fade-in </param>
        public static void Play(Sound sound, float startTime = 0f, float fadeDuration = 0f) {
            AudioSource src = ReadySource(sound);
            if (src == null) {
                Debug.LogError("AudioManager failed to play sound " + sound.name);
                return;
            } else {
                speaker = src;
            }

            speaker.time = startTime;
            if (!speaker.isPlaying) {
                speaker.Play();

                if (fadeDuration > 0) {
                    speaker.volume = 0;
                    _instance.StartCoroutine(FadeVolume(sound.Volume, fadeDuration, speaker));
                }
            }
        }

        /// <summary> 
        /// Begin playing a sound from the main source of the <c>AudioManager</c>
        /// </summary>
        /// <param name="src"> [optional] manually specifify an AudioSource to toggle </param>
        public static void PauseToggle(AudioSource src = null) {
            if (src == null) src = speaker;

            if (src.isPlaying) {
                src.Pause();
            } else {
                src.UnPause();
            }
        }

        /// <summary> 
        /// Stop the sound playing on the main source of the <c>AudioManager</c>
        /// </summary>
        /// <param name="fadeDuration"> [optional] specifify a duration for which the sound should fade-out </param>
        /// <param name="src"> [optional] manually specifify an AudioSource to fade </param>
        public static void Stop(float fadeDuration = 0f, AudioSource src = null) {
            if (src == null) src = speaker;
            if (fadeDuration <= 0) {
                src.Stop();
            } else {
                _instance.StartCoroutine(FadeOut(fadeDuration, src));
            }
        }

        #endregion //BasicFunctionality

        #region MixingRoutines

        ///<summary>
        /// Fade the volume of the main source to the <paramref name="targetVolume"/>
        ///</summary>
        /// <param name="targetVolume">target volume to fade to</param> 
        /// <param name="duration">duration of the fade event in seconds</param> 
        /// <param name="src"> [optional] manually specifify an AudioSource to fade </param>
        public static IEnumerator FadeVolume(float targetVolume, float duration, AudioSource src = null) {
            if (src == null) src = speaker;
            float initialVolume = src.volume;

            for (float elapsedTime = 0f; elapsedTime <= duration; elapsedTime += Time.deltaTime) {
                float newVolume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / duration);
                src.volume = newVolume;
                yield return null;
            }
            src.volume = targetVolume;
        }

        ///<summary>
        /// Fade the volume of the main source to zero
        ///</summary>
        /// <param name="duration">duration of the fade event in seconds</param> 
        /// <param name="src"> [optional] manually specifify an AudioSource to fade </param>
        private static IEnumerator FadeOut(float duration = 0f, AudioSource src = null) {
            AudioSource fadeOut = speaker;
            speaker = _instance.gameObject.AddComponent<AudioSource>();

            yield return _instance.StartCoroutine(FadeVolume(0f, duration, fadeOut));
            fadeOut.Stop();
            Destroy(fadeOut);
        }

        ///<summary>
        /// Cross fade to a new sound on the main source of the <c>AudioManager</c>
        ///</summary>
        /// <param name="name">the name of the new sound to be played <c>AudioManager</c></param>
        /// <param name="durationOut">duration of the fade-out event in seconds for the current sound</param> 
        /// <param name="durationIn">duration of the fade-in event in seconds for the new sound</param> 
        public static void FadeTo(string name, float durationOut, float durationIn) {
            Sound sound = GetSound(name);
            FadeTo(sound, durationOut, durationIn);
        }

        ///<summary>
        /// Cross fade to a new sound on the main source of the <c>AudioManager</c>
        ///</summary>
        /// <param name="sound">the new sound to be played </param>
        /// <param name="durationOut">duration of the fade-out event in seconds for the current sound</param> 
        /// <param name="durationIn">duration of the fade-in event in seconds for the new sound</param> 
        public static void FadeTo(Sound sound, float durationOut, float durationIn) {
            _instance.StartCoroutine(FadeToRoutine(sound, durationOut, durationIn));
        }

        private static IEnumerator FadeToRoutine(Sound sound, float durationOut, float durationIn) {
            AudioSource newSource = _instance.gameObject.AddComponent<AudioSource>();
            newSource = ReadySource(sound, newSource);
            newSource.time = speaker.time;
            newSource.volume = 0f;

            newSource.Play();
            _instance.StartCoroutine(FadeOut(durationOut));
            yield return _instance.StartCoroutine(FadeVolume(sound.Volume, durationIn, newSource));
            Destroy(speaker);
            speaker = newSource;
        }

        #endregion //MixingRoutines
    }
}