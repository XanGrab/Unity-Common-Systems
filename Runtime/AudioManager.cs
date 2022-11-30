using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public Sound[] sounds;
    public static AudioManager _instance;
    public static AudioManager Instance { get{ return _instance; } }    

    void Awake() {
        if(_instance != null && _instance != this){
            Destroy(gameObject);
            return;
        }else{
            _instance = this;
        }
        // DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.output;
        }
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name); 
        if(s == null){
            Debug.LogWarning("Sound: " + name + " not found.");
            return;
        }        
        if(s.source == null){
            Debug.LogWarning("Sound: " + s.name + " source missing.");
            return;
        }        
        //Debug.Log("Sound Played: " + name);
        if(!s.source.isPlaying){
            s.source.Play();
        }
    }
    public void Play(string name, int time) {
        Sound s = Array.Find(sounds, sound => sound.name == name); 
        if(s == null){
            Debug.LogWarning("Sound: " + s.name + " not found.");
            return;
        }        
        if(s.source == null){
            Debug.LogWarning("Sound: " + s.name + " source missing.");
            return;
        }        
        s.source.timeSamples = time;
        s.source.Play();
    }

    public void Stop(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name); 
        if(s == null){
            Debug.LogWarning("Sound: " + s.name + " not found.");
            return;
        }        
        if(s.source == null){
            Debug.LogWarning("Sound: " + s.name + " source missing.");
            return;
        }        

        s.source.Stop();
    }

    public int GetSoundTime(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name); 
        if(s == null){
            Debug.LogWarning("Sount: " + s.name + " not found.");
            return -1;
        }        
        if(s.source == null){
            Debug.LogWarning("Sound: " + s.name + " source missing.");
            return -1;
        }        

        return s.source.timeSamples;
    }

    public void Quiet(string name, bool active) {
        Sound s = Array.Find(sounds, sound => sound.name == name); 
        if(s == null){
            Debug.LogWarning("Sound: " + s.name + " not found.");
            return;
        }        
        if(s.source == null){
            Debug.LogWarning("Sound: " + s.name + " source missing.");
            return;
        }        

        if(active){
            s.source.volume *= 0.4f;
        }else{
            s.source.volume *= 1/0.4f;
        }
    }
}