using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem {
    /// <summary>
    /// Class <c>AudioManager</c> is used to trigger sounds and play music from scenes in Unity. Sounds should be manually added to the sounds array in the Inspector for use at Runtime
    /// </summary>
    [RequireComponent(typeof(AudioListener), typeof(AudioSource))]
    public class AudioManager : MonoBehaviour {
        // Singleton
        private static AudioManager _instance;
        public static AudioManager Instance { get { return _instance; } }

        private static AudioSource speaker;

        [SerializeField] private Sound[] persistSounds;
        [SerializeField] private bool playOnAwake = false;

        // Sounds that should remain persistent across the runtime of the game. These will be loaded in Awake effectivley similar to preloadAudioData

        // Manager internal array of all playable sounds
        private static List<Sound> tracks = new List<Sound>();

        /// <summary>
        /// Get the current timestamp from an AudioSource
        public static float GetTimestamp => speaker.time;

        public void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(this);
                return;
            } else {
                _instance = this;
            }

            speaker = gameObject.GetComponent<AudioSource>();

            LoadSounds(persistSounds);
            if (playOnAwake && tracks.Count > 0) Play(tracks[0].name);
        }

        #region HelperMethods

        private static Sound GetSound(string name) {
            return tracks.Find(sound => sound.name == name);
        }

        private static AudioSource ReadySource(Sound sound, AudioSource src = null) {
            if (!src) src = speaker;

            src.clip = sound.GetClip();
            src.volume = sound.Volume;
            src.pitch = sound.Pitch;
            src.loop = sound.Loop;
            src.outputAudioMixerGroup = sound.Mixer;
            return src;
        }

        public static void LoadSounds(Sound[] toLoad) {
            foreach (Sound sound in toLoad) {
                sound.LoadClips();
                tracks.Add(sound);
            }
        }

        public static void UnloadSounds(Sound[] toUnload) {
            foreach (Sound sound in toUnload) {
                sound.UnloadClips();
                tracks.Remove(sound);
            }
        }

        #endregion

        public static void PlayOnce(string name) {
            Sound sound = GetSound(name);
            AudioSource src = ReadySource(sound);
            if (!src) {
                return;
            } else {
                speaker = src;
            }
            speaker.PlayOneShot(speaker.clip);
        }

        public static void Play(string name, float startTime = 0f, float duration = 0f) {
            Sound sound = GetSound(name);
            Play(sound, startTime, duration);
        }

        public static void Play(Sound sound, float startTime = 0f, float duration = 0f) {
            AudioSource src = ReadySource(sound);
            if (!src) {
                return;
            } else {
                speaker = src;
            }

            speaker.time = startTime;
            if (!speaker.isPlaying) {
                speaker.Play();

                if (duration > 0) {
                    speaker.volume = 0;
                    _instance.StartCoroutine(FadeVolume(sound.Volume, duration, speaker));
                }
            }
        }

        public static void PauseToggle(AudioSource src = null) {
            if (!src) src = speaker;

            if (src.isPlaying) {
                src.Pause();
            } else {
                src.UnPause();
            }
        }

        public static void Stop(float duration = 0f, AudioSource src = null) {
            if (!src) src = speaker;
            if (duration <= 0) {
                src.Stop();
            } else {
                _instance.StartCoroutine(FadeOut(duration, src));
            }
        }

        private static IEnumerator FadeVolume(float targetVolume, float duration, AudioSource src = null) {
            if (!src) src = speaker;
            float initialVolume = src.volume;

            for (float elapsedTime = 0f; elapsedTime <= duration; elapsedTime += Time.deltaTime) {
                float newVolume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / duration);
                src.volume = newVolume;
                yield return null;
            }
            src.volume = targetVolume;
        }

        private static IEnumerator FadeOut(float duration = 0f, AudioSource src = null) {
            AudioSource fadeOut = speaker;
            speaker = _instance.gameObject.AddComponent<AudioSource>();

            yield return _instance.StartCoroutine(FadeVolume(0f, duration, fadeOut));
            fadeOut.Stop();
            Destroy(fadeOut);
        }

        public static void SwitchTo(string name, float durationOut, float durationIn) {
            Sound sound = GetSound(name);
            SwitchTo(sound, durationOut, durationIn);
        }

        public static void SwitchTo(Sound sound, float durationOut, float durationIn) {
            _instance.StartCoroutine(CrossFadeTo(sound, durationOut, durationIn));
        }

        private static IEnumerator CrossFadeTo(Sound sound, float durationOut, float durationIn) {
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
    }
}