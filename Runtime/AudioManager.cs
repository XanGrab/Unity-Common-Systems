using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Class <c>AudioManager</c> is used to trigger sounds and play music from scenes in Unity. Sounds should be manually added to the sounds array in the Inspector for use at Runtime
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
    // Singleton
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }    

    private AudioSource speaker;
    public Sound[] sounds;

    void Awake() {
        if(_instance != null && _instance != this){
            Destroy(gameObject);
            return;
        }else{
            _instance = this;
        }

        speaker = gameObject.GetComponent<AudioSource>();
        // gameObject.AddComponent<AudioSource>();
    }

    /// <param name="name">the name of the desired Sound</param>
    /// <returns>specified Sound from array if found; null otherwise</returns>
    private static Sound GetSound(string name) {
        return Array.Find(_instance.sounds, sound => sound.name == name); 
    }

    /// <summary> Mutator Method <c>ReadyAudio</c> prepares an AudioSource to be played in scene with the specified Sound</summary>
    /// <param name="src">the Unity AudioSource to be played from</param>
    /// <param name="name">the name of the Sound to be played</param>
    private static AudioSource ReadyAudio(AudioSource src, string name){
        Sound sound = GetSound(name);
        src ??= _instance.speaker; 
        if(sound == null){
            Debug.LogWarning("[AudioManager > Play] Sound '" + name + "' not found.");
            return null;
        }        

        src.clip = sound.clip;
        src.volume = sound.volume;
        src.outputAudioMixerGroup = sound.outputGroup;
        src.loop = sound.loop;
        return src; 
    }

    /// <summary>
    /// Method <c>PlayOnce</c> triggers a specified sound to be played in the current scene
    /// <param name="name">the name of the Sound in manager's sounds to play</param>
    /// <param name="src">the audio source to play sound from</param>
    public static void PlayOnce(AudioSource src, string name) {
        AudioSource source = ReadyAudio(src, name);
        source.Play();
    }

    /// <summary> Method <c>Play</c> is used to play a specified sound in the current scene if it is not already playing </summary>
    /// <param name="name">the name of the Sound in manager's sounds to play</param>
    /// <param name="time">playback position in seconds</param>
    /// <param name="src">the audio source to play sound from</param>
    public static void Play(string name, float time = 0f, AudioSource src = null) {
        AudioSource source = ReadyAudio(src, name);
        source.time = time;

        if(!source.isPlaying && source != null){
            source.Play();
        }
    }

    /// <summary> Method <c>Play</c> is used to stop a specified sound in the current scene </summary>
    /// <param name="name">the name of the Sound in manager's sounds to stop</param>
    /// <param name="src">the target audio source</param>
    public static void Stop(string name, AudioSource src = null) {
        AudioSource source = ReadyAudio(src, name);
        source.Stop();
    }
}