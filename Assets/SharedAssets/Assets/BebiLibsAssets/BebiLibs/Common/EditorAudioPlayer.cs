using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource))]
public class EditorAudioPlayer : MonoBehaviour
{
    public static EditorAudioPlayer Instance;
    public AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        this.audioSource = this.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Instance = this;
    }

    public static void Initialize()
    {
        if (Instance == null)
        {
            GameObject gameObject = new GameObject("Editor Audio Player Can Be Removed For Build, But Will Not Make Problems");
            EditorAudioPlayer editorAudioPlayer = gameObject.AddComponent<EditorAudioPlayer>();
        }
    }

    public static void Play(AudioClip audioClip)
    {
        Initialize();
        Instance.audioSource.clip = audioClip;
        Instance.audioSource.Play();
    }
}
