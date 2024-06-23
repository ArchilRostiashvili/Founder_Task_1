using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BebiLibs.AudioSystem
{
#if UNITY_EDITOR

    [CustomEditor(typeof(AudioTrackBaseSO), true)]
    public class AudioTrackBaseEditor : Editor
    {
        private AudioTrackBaseSO _target;

        void OnEnable()
        {
            _target = (AudioTrackBaseSO)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Play"))
                {
                    _target.Play();
                }
            }
        }
    }

#endif
}
