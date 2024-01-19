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

    // Passing in clip and importer separately as we're not completely done with the asset setup at the time we're asked to generate the preview.
    //https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/AudioClipInspector.cs
    private void DoRenderPreview(AudioClip clip, Rect wantedRect, float scaleFactor) {
        // scaleFactor *= 0.95f; // Reduce amplitude slightly to make highly compressed signals fit.
        float[] sampleData = new float[_targetClip.samples * _targetClip.channels];
        _targetClip.GetData(sampleData, 0);

        int numChannels = _targetClip.channels;
        int numSamples = (sampleData == null) ? 0 : (sampleData.Length / (2 * numChannels));
        float h = (float)wantedRect.height / (float)numChannels;

        for (int channel = 0; channel < numChannels; channel++) {
            Rect channelRect = new Rect(wantedRect.x, wantedRect.y + h * channel, wantedRect.width, h);
            Color curveColor = new Color(1.0f, 140.0f / 255.0f, 0.0f, 1.0f);

            AudioCurveRendering.AudioMinMaxCurveAndColorEvaluator dlg = delegate (float x, out Color col, out float minValue, out float maxValue) {
                col = curveColor;
                if (numSamples <= 0) {
                    minValue = 0.0f;
                    maxValue = 0.0f;
                } else {
                    float p = Mathf.Clamp(x * (numSamples - 2), 0.0f, numSamples - 2);
                    int i = (int)Mathf.Floor(p);
                    int offset1 = (i * numChannels + channel) * 2;
                    int offset2 = offset1 + numChannels * 2;
                    minValue = Mathf.Min(sampleData[offset1 + 1], sampleData[offset2 + 1]) * scaleFactor;
                    maxValue = Mathf.Max(sampleData[offset1 + 0], sampleData[offset2 + 0]) * scaleFactor;
                    if (minValue > maxValue) { float tmp = minValue; minValue = maxValue; maxValue = tmp; }
                }
            };

            AudioCurveRendering.DrawMinMaxFilledCurve(channelRect, dlg);
        }
    }

    private void PlayPreview() {
        _previewer.clip = _targetClip;
        _previewer.volume = _targetSound.Volume;
        _previewer.pitch = _targetSound.Pitch;
        _previewer.loop = _targetSound.Loop;
        _previewer.outputAudioMixerGroup = _targetSound.Mixer;

        _previewer.Play();
    }

    private void PreviewNextClip() {
        _targetClip = _targetSound.GetClip();
        // PlayPreview();
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

        // prevent default mouse events form AudioClip
        if (Event.current.isMouse) {
            return;
        }
        //TODO replace this line
        gameObjectEditor.DrawPreview(r);
        // gameObjectEditor.RenderStaticPreview();
        // Texture2D temp = AssetPreview.GetAssetPreview(_targetClip);
        // DoRenderPreview(_targetClip, r, 1.0f);
        if (_previewer.isPlaying) {
            float t = _previewer.time;

            System.TimeSpan ts = new System.TimeSpan(0, 0, 0, 0, (int)(t * 1000.0f));

            float sec2px = ((float)r.width / _targetClip.length);
            GUI.DrawTexture(new Rect(r.x + (int)(sec2px * t), r.y, 2, r.height), EditorGUIUtility.whiteTexture);
            if (r.width > 64)
                EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20), string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds));
            else
                EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20), string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds));

            gameObjectEditor.Repaint();
        }
    }

    //TODO figure out how to override click events
    // public override void OnPreviewGUI(Rect r, GUIStyle background) {
    // if (s_DefaultIcon == null) Init();

    // Event evt = Event.current;
    // if (evt.type != EventType.Repaint && evt.type != EventType.Layout && evt.type != EventType.Used) {
    //     switch (evt.type) {
    //         case EventType.MouseDrag:
    //         case EventType.MouseDown: {
    //                 if (r.Contains(evt.mousePosition)) {
    //                     var startSample = (int)(evt.mousePosition.x * (_targetClip.samples / (int)r.width));
    //                     if (!AudioUtil.IsPreviewClipPlaying() || _targetClip != m_Clip)
    //                         PlayClip(clip, startSample, s_Loop);
    //                     else
    //                         AudioUtil.SetPreviewClipSamplePosition(clip, startSample);
    //                     evt.Use();
    //                 }
    //             }
    //             break;
    //     }
    //     return;
    // }

    // if (Event.current.type == EventType.Repaint)
    //     background.Draw(r, false, false, false, false);

    // int c = _targetClip.channels;
    // Rect s_WantedRect = new Rect(r.x, r.y, r.width, r.height);
    // float sec2px = ((float)s_WantedRect.width / _targetClip.length);

    // bool previewAble = AudioUtil.HasPreview(clip) || !(AudioUtil.IsTrackerFile(clip));
    // if (!previewAble) {
    //     float labelY = (r.height > 150) ? r.y + (r.height / 2) - 10 : r.y + (r.height / 2) - 25;
    //     if (r.width > 64) {
    //         if (AudioUtil.IsTrackerFile(clip)) {
    //             EditorGUI.DropShadowLabel(new Rect(r.x, labelY, r.width, 20), string.Format("Module file with " + AudioUtil.GetMusicChannelCount(clip) + " channels."));
    //         } else
    //             EditorGUI.DropShadowLabel(new Rect(r.x, labelY, r.width, 20), "Can not show PCM data for this file");
    //     }

    //     if (m_Clip == clip && playing) {
    //         float t = AudioUtil.GetPreviewClipPosition();

    //         System.TimeSpan ts = new System.TimeSpan(0, 0, 0, 0, (int)(t * 1000.0f));

    //         EditorGUI.DropShadowLabel(new Rect(s_WantedRect.x, s_WantedRect.y, s_WantedRect.width, 20), string.Format("Playing - {0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds));
    //     }
    // } else {
    // PreviewGUI.BeginScrollView(s_WantedRect, m_Position, s_WantedRect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");

    // if (Event.current.type == EventType.Repaint) {
    //     DoRenderPreview(_targetClip, r, 1.0f);
    // }

    // for (int i = 0; i < c; ++i) {
    //     if (c > 1 && r.width > 64) {
    //         var labelRect = new Rect(s_WantedRect.x + 5, s_WantedRect.y + (s_WantedRect.height / c) * i, 30, 20);
    //         EditorGUI.DropShadowLabel(labelRect, "ch " + (i + 1));
    //     }
    // }


    // PreviewGUI.EndScrollView();
    // }

    // // force update GUI
    // if (playing)
    //     GUIView.current.Repaint();
    // }

    public override void OnPreviewSettings() {
        if (GUILayout.Button(EditorGUIUtility.IconContent("PlayButton"), EditorStyles.toolbarButton)) {
            if (!_previewer.isPlaying) {
                PlayPreview();
            } else {
                StopPreview();
            }
        };
        if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.NextKey"), EditorStyles.toolbarButton)) PreviewNextClip();
    }

    //TODO
    // public override string GetInfoString() {
    //     AudioClip clip = _targetClip;
    //     int c = _targetClip.channels;
    //     string ch = c == 1 ? "Mono" : c == 2 ? "Stereo" : (c - 1) + ".1";
    //     // AudioCompressionFormat platformFormat = AudioUtil.GetTargetPlatformSoundCompressionFormat(clip);
    //     // AudioCompressionFormat editorFormat = AudioUtil.GetSoundCompressionFormat(clip);
    //     string s = "";
    //     // if (platformFormat != editorFormat)
    //     //     s += " (" + editorFormat + " in editor" + ")";
    //     s += ", " + _targetClip.frequency + " Hz, " + ch + ", ";

    //     System.TimeSpan ts = new System.TimeSpan(0, 0, 0, 0, (int)_targetClip.length);

    //     if ((uint)_targetClip.length == 0xffffffff)
    //         s += "Unlimited";
    //     else
    //         s += String.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);

    //     return s;
    // }
}
