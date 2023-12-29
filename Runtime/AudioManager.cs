using System;
using System.Collections;
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

    private static AudioSource speaker;

    [SerializeField] private Sound[] persistSounds;
    [SerializeField] private bool playOnAwake = false;

    // Sounds that should remain persistent across the runtime of the game. These will be loaded in Awake effectivley similar to preloadAudioData

    // Manager internal array of all playable sounds
    private static Sound[] sounds;

    void Awake() {
        if(_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }else{
            _instance = this;
        }

        speaker = gameObject.GetComponent<AudioSource>();

        sounds = new Sound[persistSounds.Length];
        foreach (Sound sound in persistSounds) sound.LoadClips();
        Array.Copy(persistSounds, sounds, persistSounds.Length);

        if(playOnAwake && sounds.Length > 0) Play(sounds[0].name);
    }

    /// <param name="name">the name of the desired Sound</param>
    /// <returns>specified Sound from array if found; null otherwise</returns>
    private static Sound GetSound(string name) {
        return Array.Find(sounds, sound => sound.name == name); 
    }

    /// <summary> Mutator Method <c>ReadyAudio</c> prepares the primary AudioSource to be played in the current scene with the specified Sound</summary>
    /// <param name="name">the name of the Sound to be played</param>
    /// <param name="clip">the index of the audio clip to played in the Sound <paramref name="name"/></param>
    private static AudioSource ReadySource(string name, AudioSource src = null){
        Sound sound = GetSound(name);
        if(sound == null){
            Debug.LogError("[AudioManager > ReadySource] Sound '" + name + "' not found.");
            return null;
        }        

        if(!src) src = speaker; 

        src.clip = sound.getClip();
        src.volume = sound.Volume;
        src.pitch = sound.Pitch;
        src.loop = sound.Loop;
        src.outputAudioMixerGroup = sound.OutputMixerGroup;
        return src;
    }

    public static void LoadSounds(Sound[] toLoad) {
        foreach (Sound sound in toLoad){
            sound.LoadClips();
        }

        int prevLength = sounds.Length;
        Array.Resize(ref sounds, sounds.Length + toLoad.Length);
        Array.Copy(toLoad, 0, sounds, prevLength, toLoad.Length);
    }

    /// <summary>
    /// Get the current timestamp from an AudioSource
    public static float GetTimestamp() { return speaker.time; }

    /// <summary>
    /// Method <c>PlayOnce</c> triggers a specified sound to be played in the current scene
    /// <param name="name">the name of the Sound in manager's sounds to play</param>
    public static void PlayOnce(string name) {
        AudioSource src = ReadySource(name);
        if(!src) { 
            return;
        } else {
            speaker = src;
        }
        speaker.PlayOneShot(speaker.clip);
            Debug.Log("DEBUG [AudioManager > PlayOnce] " + name);
    }

    /// <summary> Method <c>Play</c> is used to Play a specified sound in the current scene, if it is not already playing </summary>
    /// <param name="name">the name of the Sound in manager's sounds to play</param>
    /// <param name="time">playback position in seconds</param>
    public static void Play(string name, float time = 0f) {
        AudioSource src = ReadySource(name);
        if(!src) { 
            return;
        } else {
            speaker = src;
        }

        speaker.time = time;
        if(!speaker.isPlaying) {
            Debug.Log("DEBUG [AudioManager > Play] " + name);
            speaker.Play();
        }
    }

    /// <summary> Method <c>Play</c> is used to stop a specified sound in the current scene </summary>
    public static void PauseToggle(AudioSource src = null) {
        if(!src) src = speaker; 

        if(src.isPlaying){
            src.Pause();
        }else{
            src.UnPause();
        }
    }

    /// <summary> Method <c>Play</c> is used to stop a specified sound in the current scene </summary>
    public static void Stop(AudioSource src = null) {
        if(!src) src = speaker; 
        src.Stop();
    }
    
    /// <summary> Method <c>FadeOut</c> is used to graduatley stop an audio clip</summary>
    /// <param name="step">the step by which the sound shound fade per the <paramref name="rate"/> which defaults to 1/10th of a second.<param>
    /// <param name="rate">the rate at which the volume of the playing clip should be diminished per <paramref name="step"/></param>
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

    /// <summary> Method <c>FadeIn</c> is used to graduatley start an audio clip</summary>
    /// <param name="name">the name of the Sound to fade in<param>
    /// <param name="step">the step by which the sound shound fade per the <paramref name="rate"/> which defaults to 1/10th of a second.<param>
    /// <param name="rate">the rate at which the volume of the playing clip should be diminished per <paramref name="step"/></param>
    IEnumerator FadeIn(string name, AudioSource src = null, float step = 0.1f, float rate = 0.1f) {
        if (step <= 0 || rate <= 0.1f) {
            Debug.LogError("ERROR [AudioManager > FadeOut] Invalid param step and rate must be > 0");
            yield break;
        }
        speaker = ReadySource(name);
        AudioSource source = ReadySource(name);
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

    /// <summary> Method <c>FadeIn</c> is used to graduatley transition from one audio clip to another</summary>
    /// <param name="name">the name of the Sound to cross fade to<param>
    /// <param name="step">the step by which the sound shound fade per the <paramref name="rate"/> which defaults to 1/10th of a second.<param>
    /// <param name="rate">the rate at which the volume of the playing clip should be diminished per <paramref name="step"/></param>
    IEnumerator CrossFade(string name, float step = 0.1f, float rate = 0.1f) {
        if (step <= 0 || rate <= 0.1f) {
            Debug.LogError("ERROR [AudioManager > FadeOut] Invalid param step and rate must be > 0");
            yield break;
        }
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource = ReadySource(name, newSource);
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
