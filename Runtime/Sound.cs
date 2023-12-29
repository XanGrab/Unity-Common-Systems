using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Unity Sctiptable Object <c>Sound</c> represents an audio clip to be played in game
/// </summary>
[CreateAssetMenu(menuName = "Sound")]
public class Sound : ScriptableObject {
    [SerializeField] private AudioClip[] clips;

    [SerializeField] private ClipOrder clipOrder = ClipOrder.ordered; 

    [SerializeField] private AudioMixerGroup outputGroup;
    public AudioMixerGroup OutputMixerGroup => outputGroup;

    private int clipIndex = -1;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0f;
    public float Volume => volume;

    [Range(-10, 10)]
    [SerializeField] private int pitch = 0;
    public int Pitch => pitch;

    [SerializeField] private bool loop = false;
    public bool Loop => loop;

    public void LoadClips(){
        foreach(AudioClip clip in clips) {
            clip.LoadAudioData();
        }
    }

    public void setClipIndex(int index){
        if (clipOrder != ClipOrder.manual) {
            Debug.LogWarning("Warning [Sound " + name + "] should not call setClipIndex unless ClipOrder is set to manual");
        }

        clipIndex = index;
    }

    public AudioClip getClip() {
        switch(clipOrder) {
            case ClipOrder.ordered:
                clipIndex = (clipIndex + 1) % clips.Length;
                break;
            case ClipOrder.random:
                clipIndex = Random.Range(0, clips.Length);
                break;
            default:
                break;
        }
        return clips[clipIndex];
    }

    public AudioClip getClip(int index) { return clips[index]; }
}

enum ClipOrder {
    ordered = 0,
    random = 1,
    manual = 2,
}

