using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem {
    /// <summary>
    /// Class <c>AudioManager</c> is used to trigger sounds and play music from scenes in Unity. Sounds should be manually added to the sounds array in the Inspector for use at Runtime
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour {
        // Singleton
        private static AudioManager _instance;
        public static AudioManager Instance { get { return _instance; } }    

        private static AudioSource speaker;

        [SerializeField] private Sound[] persistSounds;
        [SerializeField] private bool playOnAwake = false;

        // Sounds that should remain persistent across the runtime of the game. These will be loaded in Awake effectivley similar to preloadAudioData

        // Manager internal array of all playable sounds
        private static Sound[] tracks;

        void Awake() {
            if(_instance != null && _instance != this) {
                Destroy(this);
                return;
            }else{
                _instance = this;
            }

            speaker = gameObject.GetComponent<AudioSource>();

            tracks = new Sound[persistSounds.Length];
            foreach (Sound sound in persistSounds) sound.LoadClips();
            Array.Copy(persistSounds, tracks, persistSounds.Length);

            if(playOnAwake && tracks.Length > 0) Play(tracks[0].name);
        }

        #region HelperMethods

        private static Sound GetSound(string name) {
            return Array.Find(tracks, sound => sound.name == name); 
        }

        private static AudioSource ReadySource(Sound sound, AudioSource src = null){
            if(!src) src = speaker; 

            src.clip = sound.getClip();
            src.volume = sound.Volume;
            src.pitch = sound.Pitch;
            src.loop = sound.Loop;
            src.outputAudioMixerGroup = sound.Mixer;
            return src;
        }

        /// <summary>
        /// Get the current timestamp from an AudioSource
        public static float GetTimestamp() { return speaker.time; }

        public static void LoadSounds(Sound[] toLoad) {
            foreach (Sound sound in toLoad){
                sound.LoadClips();
            }

            int prevLength = tracks.Length;
            Array.Resize(ref tracks, tracks.Length + toLoad.Length);
            Array.Copy(toLoad, 0, tracks, prevLength, toLoad.Length);
        }

        #endregion

        public static void PlayOnce(string name) {
            Sound sound = GetSound(name);
            AudioSource src = ReadySource(sound);
            if(!src) { 
                return;
            } else {
                speaker = src;
            }
            speaker.PlayOneShot(speaker.clip);
                Debug.Log("DEBUG [AudioManager > PlayOnce] " + name);
        }

        public static void Play(string name, float fadeTime = 0f, float time = 0f) {
            Sound sound = GetSound(name);
            AudioSource src = ReadySource(sound);
            if(!src) { 
                return;
            } else {
                speaker = src;
            }

            speaker.time = time;
            if(!speaker.isPlaying) {
                speaker.Play();
                Debug.Log("DEBUG [AudioManager > Play] " + name);
            }
        }

        public static void Play(Sound sound, float fadeTime = 0f, float time = 0f) {
            AudioSource src = ReadySource(sound);
            if(!src) { 
                return;
            } else {
                speaker = src;
            }

            speaker.time = time;
            if(!speaker.isPlaying) {
                speaker.Play();
                Debug.Log("DEBUG [AudioManager > Play] " + sound.name);
            }
        }

        public static void PauseToggle(AudioSource src = null) {
            if(!src) src = speaker; 

            if(src.isPlaying){
                src.Pause();
            }else{
                src.UnPause();
            }
        }

        public static void Stop(AudioSource src = null) {
            if(!src) src = speaker; 
            src.Stop();
        }
        
        IEnumerator FadeOut(AudioSource src = null, float step = 0.1f, float rate = 0.1f) {
            if (step <= 0) {
                Debug.LogError("ERROR [AudioManager > FadeOut] Invalid param step and rate must be > 0");
                yield break;
            }
            float clipVolume = speaker.volume;

            for (float vol = clipVolume; vol > 0; vol -= rate){
                speaker.volume = vol;
                yield return new WaitForSeconds(step);
            }
        }

        IEnumerator FadeIn(string name, AudioSource src = null, float step = 0.1f, float rate = 0.1f) {
            if (step <= 0 || rate <= 0.1f) {
                Debug.LogError("ERROR [AudioManager > FadeOut] Invalid param step and rate must be > 0");
                yield break;
            }
            Sound sound = GetSound(name);
            speaker = ReadySource(sound);
            AudioSource source = ReadySource(sound);
            if(!source) { 
                yield break;
            } else {
                speaker = src;
            }

            float clipVolume = speaker.volume;

            for (float vol = 0.0f; vol < clipVolume; vol += rate){
                speaker.volume = vol;
                yield return new WaitForSeconds(step);
            }
        }

        IEnumerator CrossFade(string name, float step = 0.1f, float rate = 0.1f) {
            if (step <= 0 || rate <= 0.1f) {
                Debug.LogError("ERROR [AudioManager > FadeOut] Invalid param step and rate must be > 0");
                yield break;
            }
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            Sound sound = GetSound(name);
            newSource = ReadySource(sound, newSource);
            newSource.time = speaker.time;
            newSource.volume = 0f;
            float clipVolume = speaker.volume;

            for (float vol = clipVolume; vol > 0; vol -= rate){
                speaker.volume = vol;
                newSource.volume += rate;
                yield return new WaitForSeconds(step);
            }

            Destroy(speaker);
            speaker = newSource;
        }
    }
}