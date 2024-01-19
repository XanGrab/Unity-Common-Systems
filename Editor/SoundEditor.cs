using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using SoundSystem;

[CustomEditor(typeof(Sound))]
public class SoundEditor : Editor {
    private Sound _targetSound;
    private AudioClip _targetClip;
    private AudioSource _previewer;

    public void Awake() {
        Debug.Log("Awake");
        _targetSound = (Sound)target;
        _targetClip = _targetSound.GetClip();
    }

    private void OnEnable() {
        _previewer = EditorUtility
            .CreateGameObjectWithHideFlags("AudioPreview", HideFlags.HideAndDontSave,
                typeof(AudioSource))
            .GetComponent<AudioSource>();
    }

    private void OnDisable() {
        DestroyImmediate(_previewer.gameObject);
    }

    private void PlayPreview() {
        _targetClip = _targetSound.GetClip();
        _previewer.clip = _targetClip;
        _previewer.volume = _targetSound.Volume;
        _previewer.pitch = _targetSound.Pitch;
        _previewer.loop = _targetSound.Loop;
        _previewer.outputAudioMixerGroup = _targetSound.Mixer;

        _previewer.Play();
    }

    private void StopPreview() {
        _previewer.Stop();
    }

    public override bool HasPreviewGUI() {
        return _targetClip != null;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background) {
        Editor gameObjectEditor = null;
        if (!gameObjectEditor) {
            gameObjectEditor = Editor.CreateEditor(_targetClip);
        }
        gameObjectEditor.OnPreviewGUI(r, background);

        TimeSpan timespan = TimeSpan.FromSeconds(_targetClip.length);
        String clipLengthf = _targetClip.length > 3600 ? timespan.ToString(@"hh\:mm\:ss\.fff") : timespan.ToString(@"mm\:ss\.fff");
        EditorGUI.DropShadowLabel(r, _targetClip.name + ", " + clipLengthf);
    }

    public override void OnPreviewSettings() {
        if (GUILayout.Button(EditorGUIUtility.IconContent("PlayButton"), EditorStyles.toolbarButton)) {
            if (!_previewer.isPlaying) {
                PlayPreview();
            } else {
                StopPreview();
            }
        };
    }
}
