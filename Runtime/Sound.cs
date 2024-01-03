using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem {

    enum ClipType {
        manual = 0,
        ordered = 1,
        random = 2,
        additive = 3,
    }

    /// <summary>
    /// Unity Sctiptable Object <c>Sound</c> represents an audio clip to be played in game
    /// </summary>
    [CreateAssetMenu(menuName = "Sound")]
    public class Sound : ScriptableObject {
        [SerializeField] private AudioClip[] clips;

        [SerializeField] private ClipType clipType = ClipType.manual;

        [SerializeField] private AudioMixerGroup _mixer;
        public AudioMixerGroup Mixer => _mixer;

        private int clipIndex = 0;

        [Range(0f, 1f)]
        [SerializeField] private float volume = 0f;
        public float Volume => volume;

        [Range(-10, 10)]
        [SerializeField] private int pitch = 1;
        public int Pitch => pitch;

        [SerializeField] private bool loop = false;
        public bool Loop => loop;

        public void OnEnable() {
            AudioManager.LoadSounds(new Sound[] { this });
        }

        public void OnDisable() {
            AudioManager.UnloadSounds(new Sound[] { this });
        }

        #region ManageClips

        public void LoadClips() {
            foreach (AudioClip clip in clips) {
                clip.LoadAudioData();
            }
        }
        public void UnloadClips() {
            foreach (AudioClip clip in clips) {
                clip.UnloadAudioData();
            }
        }

        public AudioClip GetClip(int index) { return clips[index]; }

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

        public void SetClipIndex(int index) {
            if (clipType != ClipType.manual) {
                Debug.LogWarning("Warning [Sound " + name + "] should not call setClipIndex unless ClipOrder is set to manual");
            }

            clipIndex = index;
        }


        #endregion //ManageClips

        #region AudioCtrl

        public void Play(float startTime = 0f, float duration = 0f) {
            AudioManager.Play(this, startTime, duration);
        }

        public void PauseToggle() {
            AudioManager.PauseToggle();
        }

        public void Stop(float duration = 0f) {
            AudioManager.Stop(duration);
        }

        #endregion //AudioCtrl
    }
}