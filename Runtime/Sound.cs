
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Unity Sctiptable Object <c>Sound</c> represents an audio clip to be played in game
/// </summary>
[CreateAssetMenu(menuName = "Sound")]
public class Sound : ScriptableObject {

    public string name;

    public AudioClip clip;

    public AudioMixerGroup outputGroup;

    [Range(0f, 1f)]
    public float volume = 0f;

    public bool loop;

#if UNITY_EDITOR
// TODO Editor Preview
//     #region Preview

//     private AudioSource previewer;

//     private void OnEnable() {
//         previewer = EditorUtility.createObjectWithHideFlags("AudioPreview",
//         HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();    
//     }

//     private void OnDisable() {
//         DestroyImmediate(previewer.gameObject);
//     }

//     private void PlayPreview(){
//         if(previewer.isPlaying){
//             previewer.Stop();
//         } else {
//             previewer.Play();
//         }
//     }

//     public override void OnInsepectorGUI(){
//         DrawDefaultInsepctor();
//     }
//     #endregion
#endif
}