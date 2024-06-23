using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnabler : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void OnEnable()
    {
        _audioSource.Play();
    }

    private void OnDisable()
    {
        _audioSource.Pause();
    }
}
