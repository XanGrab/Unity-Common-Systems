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

        [SerializeField] private ClipType clipType = ClipType.ordered; 

        [SerializeField] private AudioMixerGroup _mixer;
        public AudioMixerGroup Mixer => _mixer;

        private int clipIndex = 0;

        [Range(0f, 1f)]
        [SerializeField] private float volume = 0f;
        public float Volume => volume;

        [Range(-10, 10)]
        [SerializeField] private int pitch = 0;
        public int Pitch => pitch;

        [SerializeField] private bool loop = false;
        public bool Loop => loop;

        #region ManageClips

        public void LoadClips(){
            foreach(AudioClip clip in clips) {
                clip.LoadAudioData();
            }
        }

        public void setClipIndex(int index){
            if (clipType != ClipType.manual) {
                Debug.LogWarning("Warning [Sound " + name + "] should not call setClipIndex unless ClipOrder is set to manual");
            }

            clipIndex = index;
        }

        public AudioClip getClip() {
            switch(clipType) {
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

        public AudioClip getClip(int index) { return clips[index]; }

        #endregion //ManageClips

        #region AudioCtrl

        public void Play(float fadeTime, float time) {
            AudioManager.Play(this, fadeTime, time);
        }

        public void PauseToggle() {
            AudioManager.PauseToggle();
        }

        public void Stop() {
            AudioManager.Stop();
        }
        #endregion //AudioCtrl
    }
}