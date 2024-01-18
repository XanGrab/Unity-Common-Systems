using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem {

    /// <summary> 
    /// Specifies how clips should be retrieved from a Sound
    /// </summary>
    enum ClipType {
        ///<summary>call <see cref="SetClipIndex"/> before getting the next clip</summary>
        manual = 0,
        ///<summary>automatically switch to the next clip; looping back to the start at the end of the array</summary>
        ordered = 1,
        ///<summary>randomly select the next clip</summary>
        random = 2,
        ///<summary>TODO treat each clip as an addative track on the same sound</summary>
        additive = 3,
    }

    /// <summary>
    /// <c>Sound</c> is a SctiptableObject which represents represents a collection of AudioClips for a specific sound to be played in game
    /// </summary>
    [CreateAssetMenu(menuName = "Sound")]
    public class Sound : ScriptableObject {
        /// <summary> 
        /// An internal array of all playable clips
        /// </summary>
        [SerializeField] private AudioClip[] clips;

        /// <summary> 
        /// Specifies how clips should be retrieved from this Sound
		/// <cref name="ClipType"/>
        /// </summary>
        [SerializeField] private ClipType clipType = ClipType.manual;

        ///<summary>
        /// the output <c>AudioMixerGroup</c> this Sound should be played on
        ///</summary>
        [SerializeField] private AudioMixerGroup _mixer;
        public AudioMixerGroup Mixer => _mixer;

        private int clipIndex = 0;

        ///<summary>
        /// the volume this Sound should be played at
        ///</summary>
        [Range(0f, 1f)]
        [SerializeField] private float volume = 0f;
        public float Volume => volume;

        ///<summary>
        /// the pitch this Sound should be played at
        ///</summary>
        [Range(-10, 10)]
        [SerializeField] private int pitch = 1;
        public int Pitch => pitch;

        [SerializeField] private bool loop = false;
        public bool Loop => loop;

        public void OnEnable() {
            if (clips != null) LoadClips();
            AudioManager.LoadSounds(new Sound[] { this });
        }

        public void OnDisable() {
            if (clips != null) UnloadClips();
            AudioManager.UnloadSounds(new Sound[] { this });
        }

        #region ManageClips

        private void LoadClips() {
            foreach (AudioClip clip in clips) {
                clip.LoadAudioData();
            }
        }

        private void UnloadClips() {
            foreach (AudioClip clip in clips) {
                clip.UnloadAudioData();
            }
        }

        ///<summary>
        /// Manually get the AudioClip at <paramref name="index"/>
        ///</summary>
        public AudioClip GetClip(int index) { return clips[index]; }

        ///<summary>
        /// Get the next AudioClip for this sound
        ///</summary>
        public AudioClip GetClip() {
            switch (clipType) {
                case ClipType.ordered:
                    clipIndex = (clipIndex + 1) % clips.Length;
                    break;
                case ClipType.random:
                    clipIndex = Random.Range(0, clips.Length);
                    break;
                default:
                    break;
            }
            return clips[clipIndex];
        }

        ///<summary>
        /// Manually set the clip index to <paramref name="index"/>
        ///</summary>
        public void SetClipIndex(int index) {
            if (clipType != ClipType.manual) {
                Debug.LogWarning("Warning [Sound " + name + "] should not call setClipIndex unless ClipOrder is set to manual");
            }

            clipIndex = index;
        }


        #endregion //ManageClips

        #region AudioCtrl

        /// <summary> 
        /// Play a one shot sound effect on the <c>AudioManager</c>
        /// </summary>
        public void PlayOneShot() {
            AudioManager.PlayOneShot(this);
        }


        /// <summary> 
        /// Begin playing this sound from the main source of the <c>AudioManager</c>
        /// </summary>
        /// <param name="startTime"> [optional] the starting timestamp of the clip </param>
        /// <param name="fadeDuration"> [optional] specifify a duration for which the sound should fade-in </param>
        public void Play(float startTime = 0f, float duration = 0f) {
            AudioManager.Play(this, startTime, duration);
        }

        #endregion //AudioCtrl
    }
}